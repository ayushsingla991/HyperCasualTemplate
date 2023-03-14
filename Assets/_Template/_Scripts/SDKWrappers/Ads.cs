using UnityEngine;
using UnityEngine.SceneManagement;

public class Ads : MonoBehaviour {

    public static Ads instance;

    bool _adsRemoved;
    public static bool adsRemoved {
        private set {
            instance._adsRemoved = value;
        }
        get {
            return instance._adsRemoved;
        }
    }

    public static int firstBannerAdLevel;
    public static int firstIntAdLevel;
    public static float firstAdTime;
    public static float adInterval;
    public static float lastAdTime = -15;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;

        adsRemoved = PlayerPrefs.GetInt(K.Prefs.AdsRemoved, 0) == 1;
    }

    void SetRemote() {
        firstBannerAdLevel = FBAnalytics.GetNumber(K.RemoteConfig.first_banner_ad_level);
        firstIntAdLevel = FBAnalytics.GetNumber(K.RemoteConfig.first_int_ad_level);
        adInterval = FBAnalytics.GetNumber(K.RemoteConfig.ad_interval);
        firstAdTime = FBAnalytics.GetNumber(K.RemoteConfig.first_ad_time);
    }

    void Start() {
        SetRemote();
        FBAnalytics.AddOnDataFetch(SetRemote);
        lastAdTime = firstAdTime - adInterval;
        IAP.AddOnItemPurchaseListener((string productId, string receipt) => {
            if (productId == K.IAP.remove_ads) {
                RemoveAds();
            }
        });
        if (adsRemoved) {
            HideBanner("Ad Removal Check");
        }
        if (LevelManager.Level() + 1 < firstBannerAdLevel) {
            HideBanner("Lower Levels");
        }
        SceneManager.activeSceneChanged += (_origin, _dest) => {
            if (adsRemoved) {
                if (LevelManager.Level() + 1 >= firstBannerAdLevel) {
                    ShowBanner();
                }
            }
        };
    }

    public static void GDPR(bool status) {
        IronSourceManager.GDPR(status);
    }

    void RemoveAds() {
        adsRemoved = true;
        PlayerPrefs.SetInt(K.Prefs.AdsRemoved, adsRemoved ? 1 : 0);
        HideBanner("Remove Ads Callback");
    }

    public static bool ShowInterstitial(string placementId, System.Action _OnInterstitialClosed) {
        if (adsRemoved) {
            return false;
        }
        if (LevelManager.Level() + 1 < firstIntAdLevel) {
            return false;
        }
        if (Application.isEditor) {
            if (_OnInterstitialClosed != null) {
                _OnInterstitialClosed();
            }
            return true;
        } else {
            return IronSourceManager.ShowInterstitial(placementId, _OnInterstitialClosed);
        }
    }

    public static bool ShowRewardAd(System.Action<bool> _OnRvRewarded) {
        if (Application.isEditor) {
            _OnRvRewarded(true);
            return true;
        } else {
            return IronSourceManager.ShowRewardAd(_OnRvRewarded);
        }
    }
    public static bool IsRewardAdLoaded() {
        return IronSourceManager.IsRewardAdLoaded();
    }

    public static void AddOnIntLoaded(System.Action<bool> _OnIntLoaded) {
        IronSourceManager.AddOnIntLoaded(_OnIntLoaded);
    }

    public static void RemoveOnIntLoaded(System.Action<bool> _OnIntLoaded) {
        IronSourceManager.RemoveOnIntLoaded(_OnIntLoaded);
    }

    public static void OnRVLoaded(System.Action<bool> _OnRVLoaded) {
        IronSourceManager.AddOnRvLoaded(_OnRVLoaded);
    }
    public static void RemoveOnRVLoaded(System.Action<bool> _OnRVLoaded) {
        IronSourceManager.RemoveOnRvLoaded(_OnRVLoaded);
    }
    public static void HideBanner(string calledFrom) {
        IronSourceManager.HideBanner(calledFrom);
    }
    public static void ShowBanner() {
        IronSourceManager.ShowBanner();
    }

}