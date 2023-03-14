// #define Ironsource
using UnityEngine;

public class IronSourceManager : MonoBehaviour {

    public static IronSourceManager instance;
    bool init;

    System.Action OnInterstitialClosed;
    System.Action<bool> OnRVRewarded;
    System.Action<bool> OnRVLoaded;
    System.Action<bool> OnIntLoaded;

    bool hideBanner;
    bool rewarded;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        init = true;
        hideBanner = false;
    }

    void Start() {
#if Ironsource
        if (!Ads.adsRemoved) {
            IronSource.Agent.init(Ads.IRONSOURCE_ID, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
        }

        if (!Ads.adsRemoved) {
            IronSourceEvents.onInterstitialAdOpenedEvent += IntShown;
            IronSourceEvents.onInterstitialAdClosedEvent += IntClosed;
            IronSourceEvents.onInterstitialAdReadyEvent += IntLoaded;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += IntFailedToLoad;
            IronSourceEvents.onInterstitialAdClickedEvent += IntClicked;

            IronSourceEvents.onBannerAdLoadedEvent += OnBannerLoad;
            IronSourceEvents.onBannerAdLoadFailedEvent += OnBannerFailedToLoad;

            RequestBanner();
            RequestInt();
        }

        IronSourceEvents.onRewardedVideoAdOpenedEvent += RVOpened;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RVDone;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RVClosed;
        IronSourceEvents.onRewardedVideoAdClickedDemandOnlyEvent += RVClicked;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RVLoaded;
#endif
    }

    void RequestBanner() {
        Debug.Log("Ironsource Loading Banner");
#if Ironsource
        IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
#endif
        FBAnalytics.LogEvent(K.Event.banner_requested);
    }

    void OnBannerLoad() {
#if Ironsource
        if (hideBanner) {
            IronSource.Agent.hideBanner();
        } else {
            FBAnalytics.LogEvent(K.Event.banner_loaded);
            IronSource.Agent.displayBanner();
        }
#endif
    }

#if Ironsource
    void OnBannerFailedToLoad(IronSourceError error) {
        Debug.Log("Ironsource Banner Failed to Load: " + error.getDescription() + ", Code: " + error.getCode());
    }
#endif

    void RequestInt() {
#if Ironsource
        IronSource.Agent.loadInterstitial();
        FBAnalytics.LogEvent(K.Event.interstitial_requested);
#endif
    }

    public static void GDPR(bool status) {
#if Ironsource
        IronSource.Agent.setConsent(status); // ironsource
#endif
    }

    public static bool ShowInterstitial(string placement, System.Action _OnInterstitialClosed) {
        if (instance == null) {
            return false;
        }
        if (Ads.adsRemoved) {
            return false;
        }
        if (_OnInterstitialClosed != null) {
            instance.OnInterstitialClosed = _OnInterstitialClosed;
        }
        Debug.Log("Ad: Time.time - lastAdTime >= instance.adInterval : " + Time.time + " - " + Ads.lastAdTime + " >= " + Ads.adInterval);
#if Ironsource
        if (Time.time - Ads.lastAdTime >= Ads.adInterval) {
            if (IronSource.Agent.isInterstitialReady()) {
                FBAnalytics.LogEvent(K.Event.interstitial_requested_to_be_shown);
                if (placement == "") {
                    IronSource.Agent.showInterstitial();
                } else {
                    if (IronSource.Agent.isInterstitialPlacementCapped(placement)) {
                        IronSource.Agent.showInterstitial();
                    } else {
                        IronSource.Agent.showInterstitial(placement);
                    }
                }
                Ads.lastAdTime = Time.time;
                return true;
            } else {
                instance.RequestInt();
            }
        }
#endif
        return false;
    }

    public void IntShown() {
        FBAnalytics.LogEvent(K.Event.interstitial_shown);
    }

    public void IntClosed() {
        if (instance.OnInterstitialClosed != null) {
            instance.OnInterstitialClosed();
        }
        FBAnalytics.LogEvent(K.Event.interstitial_closed);
#if Ironsource
        if (!IronSource.Agent.isInterstitialReady()) {
            instance.RequestInt();
        }
#endif
    }

    public void IntLoaded() {
        FBAnalytics.LogEvent(K.Event.interstitial_loaded);
        if (OnIntLoaded != null) {
            OnIntLoaded(true);
        }
    }

#if Ironsource
    public void IntFailedToLoad(IronSourceError error) {
        Debug.Log("Ironsource Int Failed to Load: " + error.getDescription() + ", Code: " + error.getCode());
        FBAnalytics.LogEvent(K.Event.interstitial_failed_to_load);
        if (OnIntLoaded != null) {
            OnIntLoaded(false);
        }
    }
#endif

    public void IntClicked() {
        FBAnalytics.LogEvent(K.Event.interstitial_clicked);
    }

    public static bool ShowRewardAd(System.Action<bool> _OnRVRewarded) {
        if (instance == null) {
            return false;
        }
        instance.OnRVRewarded = _OnRVRewarded;
        if (Application.isEditor) {
            if (instance.OnRVRewarded != null)instance.OnRVRewarded(true);
            return true;
        }
#if Ironsource
        if (IronSource.Agent.isRewardedVideoAvailable()) {
            instance.rewarded = false;
            IronSource.Agent.showRewardedVideo();
            return true;
        }
#endif
        return false;
    }

    public static bool IsRewardAdLoaded() {
        if (instance == null) {
            return false;
        }
        if (Application.isEditor) {
            return true;
        }
#if Ironsource
        return IronSource.Agent.isRewardedVideoAvailable();
#endif
        return false;
    }

    public void RVOpened() {
        FBAnalytics.LogEvent(K.Event.rewarded_video_shown);
    }

    public void RVClicked(string clicked) {
        RewardDone();
    }

#if Ironsource
    public void RVDone(IronSourcePlacement placement) {
        RewardDone();
    }
#endif

    void RVClosed() {
        if (rewarded) {
            if (OnRVRewarded != null) {
                OnRVRewarded(true);
            }
            return;
        }

        if (OnRVRewarded != null) {
            OnRVRewarded(false);
        }

        FBAnalytics.LogEvent(K.Event.rewarded_closed_no_reward);

        Ads.lastAdTime = Time.time;
        rewarded = false;
    }

    void RVLoaded(bool rvLoaded) {
        if (OnRVLoaded != null) {
            OnRVLoaded(rvLoaded);
        }
    }

    void RewardDone() {
        if (rewarded) {
            return;
        }
        rewarded = true;
        Ads.lastAdTime = Time.time;
    }

    public static void AddOnIntLoaded(System.Action<bool> _OnIntLoaded) {
        instance.OnIntLoaded += _OnIntLoaded;
    }

    public static void RemoveOnIntLoaded(System.Action<bool> _OnIntLoaded) {
        instance.OnIntLoaded -= _OnIntLoaded;
    }

    public static void AddOnRvLoaded(System.Action<bool> _OnRVLoaded) {
        instance.OnRVLoaded += _OnRVLoaded;
    }

    public static void RemoveOnRvLoaded(System.Action<bool> _OnRVLoaded) {
        instance.OnRVLoaded -= _OnRVLoaded;
    }

    public static void HideBanner(string calledFrom) {
        if (instance == null) {
            return;
        }
        Debug.Log("Hide Banner Ad: " + calledFrom);
        instance.hideBanner = true;
#if Ironsource
        IronSource.Agent.hideBanner();
#endif
    }

    public static void ShowBanner() {
        if (instance == null) {
            return;
        }
        if (Ads.adsRemoved) {
            return;
        }
        Debug.Log("Show Banner Ad");
        instance.hideBanner = false;
#if Ironsource
        IronSource.Agent.displayBanner();
#endif
    }

    void OnDestroy() {
        if (!init) {
            return;
        }
#if Ironsource
        IronSource.Agent.destroyBanner();
#endif
    }

    void OnApplicationPause(bool pauseStatus) {
#if Ironsource
        IronSource.Agent.onApplicationPause(pauseStatus);
#endif
    }
}