// #define FirebaseAnalytics
// #define FirebaseRemoteConfig
#if FirebaseAnalytics
using Firebase;
using Firebase.Analytics;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FBAnalytics : MonoBehaviour {

    static FBAnalytics instance;

    Dictionary<string, object> defaults = new Dictionary<string, object>();

    bool initialised;

    System.Action OnDataFetch;

    List<string> queuedEvents;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;

        SetupDefaults();

        queuedEvents = new List<string>();

#if FirebaseAnalytics
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                SetupRemoteConfig();
            } else {
                UnityEngine.Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
#endif
    }

    public static void GDPR(bool status) {
#if FirebaseAnalytics
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(status);
#endif
    }

    void SetupDefaults() {
        defaults = new Dictionary<string, object>();
        defaults.Add(K.RemoteConfig.first_banner_ad_level, 1);
        defaults.Add(K.RemoteConfig.first_int_ad_level, 1);
        defaults.Add(K.RemoteConfig.first_ad_time, 15);
        defaults.Add(K.RemoteConfig.ad_interval, 30);
    }

    void SetupRemoteConfig() {
#if FirebaseRemoteConfig
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);

        Firebase.RemoteConfig.ConfigSettings cs = new Firebase.RemoteConfig.ConfigSettings();
        if (Debug.isDebugBuild) {
            cs.MinimumFetchInternalInMilliseconds = 0;
        }

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(cs);

        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync();
        fetchTask.ContinueWith(FetchComplete);
#endif
    }

    void FetchComplete(Task fetchTask) {
        if (fetchTask.IsCanceled) {
            Debug.Log("Fetch canceled.");
        } else if (fetchTask.IsFaulted) {
            Debug.Log("Fetch encountered an error.");
        } else if (fetchTask.IsCompleted) {
            Debug.Log("Fetch completed successfully!");
            Debug.Log("data fetch callback null: " + (OnDataFetch == null));
            initialised = true;
            if (queuedEvents.Count > 0) {
                for (int i = 0; i < queuedEvents.Count; i++) {
                    LogEvent(queuedEvents[i]);
                }
                queuedEvents.Clear();
            }
            if (OnDataFetch != null) {
                OnDataFetch();
            }
        }

#if FirebaseRemoteConfig
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;

        switch (info.LastFetchStatus) {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason) {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
#endif
    }

    public static string GetString(string key) {
        string remoteValue = "";
        if (instance.initialised) {
#if FirebaseRemoteConfig
            remoteValue = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
#endif
            PlayerPrefs.SetString(key, remoteValue);
            Debug.Log("Firebase Remote Key: " + key + " fetched value: " + remoteValue);
        } else {
            remoteValue = PlayerPrefs.GetString(key, instance.defaults[key] as string);
            Debug.Log("Firebase Remote Key: " + key + " saved value: " + remoteValue);
        }
        return remoteValue;
    }

    public static int GetNumber(string key) {
        int remoteValue = 0;
        if (instance.initialised) {
#if FirebaseRemoteConfig
            remoteValue = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
#endif
            PlayerPrefs.SetInt(key, remoteValue);
            Debug.Log("Firebase Remote Key: " + key + " fetched value: " + remoteValue);
        } else {
            remoteValue = PlayerPrefs.GetInt(key, (int)instance.defaults[key]);
            Debug.Log("Firebase Remote Key: " + key + " saved value: " + remoteValue);
        }
        return remoteValue;
    }

    public static bool GetBoolean(string key) {
        bool remoteValue = false;
        if (instance.initialised) {
#if FirebaseRemoteConfig
            remoteValue = (bool)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
#endif
            PlayerPrefs.SetInt(key, remoteValue ? 1 : 0);
            Debug.Log("Firebase Remote Key: " + key + " fetched value: " + remoteValue);
        } else {
            remoteValue = PlayerPrefs.GetInt(key, (bool)instance.defaults[key] ? 1 : 0) == 1;
            Debug.Log("Firebase Remote Key: " + key + " saved value: " + remoteValue);
        }
        return remoteValue;
    }

    public static void AddOnDataFetch(System.Action _OnDataFetch) {
        instance.OnDataFetch += _OnDataFetch;
    }

    public static void RemoveOnDataFetch(System.Action _OnDataFetch) {
        instance.OnDataFetch -= _OnDataFetch;
    }

    public static void LogEvent(string eventName, string payload, string payloadValue) {
        if (!instance.initialised) {
            instance.queuedEvents.Add(eventName);
            return;
        }
        Debug.Log("Firebase Log:: " + eventName);

        Dictionary<string, string> param = new Dictionary<string, string>() { { payload, payloadValue } };
        LogEvent(eventName, param);
    }

    public static void LogEvent(string eventName, Dictionary<string, string> payload = null) {
        if (!instance.initialised) {
            instance.queuedEvents.Add(eventName);
            return;
        }
        Debug.Log("Firebase Log:: " + eventName);
#if FirebaseAnalytics
        if (payload == null) {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        } else {
            Firebase.Analytics.Parameter[] parameters = new Firebase.Analytics.Parameter[payload.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> pl in payload) {
                parameters[i] = new Firebase.Analytics.Parameter(pl.Key, pl.Value);
                i++;
            }
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameters);
        }
#endif
    }

}