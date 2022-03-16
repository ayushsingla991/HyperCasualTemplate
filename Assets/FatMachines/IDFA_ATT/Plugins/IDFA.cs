using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using AOT;
using System.Collections.Generic;

public class IDFA : MonoBehaviour {

    public enum Status { DENIED, AUTHORIZED, NOT_DETERMINED, RESTRICTED }

    static IDFA instance;

#if UNITY_IOS
    private delegate void DelegateIDFAStatus(int status);

    [DllImport("__Internal")]
    private static extern void _RequestIDFA(DelegateIDFAStatus callback);

    [MonoPInvokeCallback(typeof(DelegateIDFAStatus))]
    private static void IDFAStatus(int status) {
        Debug.Log("IDFA Status: " + status);
        if (OnIDFAStatus != null) {
            if (status == 0) {
                OnIDFAStatus(Status.DENIED);
            } else if (status == 1) {
                OnIDFAStatus(Status.AUTHORIZED);
            } else if (status == 2) {
                OnIDFAStatus(Status.NOT_DETERMINED);
            } else if (status == 3) {
                OnIDFAStatus(Status.RESTRICTED);
            }
        }
    }
#endif

    public static string __idfaDesc = "This identifier will be used to deliver personalized ads to you.";
    public string _idfaDesc = __idfaDesc;
    public static List<string> __sdkAdIds = new List<string>();
    public List<string> _sdkAdIds = __sdkAdIds;

    public static string idfaDesc {
        get {
            if (instance != null) {
                return instance._idfaDesc;
            } else {
                return __idfaDesc;
            }
        }
        set {
            if (instance != null) {
                instance._idfaDesc = value;
            } else {
                __idfaDesc = value;
            }
        }
    }

    public static List<string> sdkAdIds {
        get {
            if (instance != null) {
                return instance._sdkAdIds;
            } else {
                return __sdkAdIds;
            }
        }
        set {
            if (instance != null) {
                instance._sdkAdIds = value;
            } else {
                __sdkAdIds = value;
            }
        }
    }

    static System.Action<Status> OnIDFAStatus;

    private void Awake() {
        instance = this;
    }

    public static void RequestPopup(System.Action<Status> _OnIDFAStatus = null) {
        OnIDFAStatus = _OnIDFAStatus;
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor) {
            return;
        }
        string[] versions = Device.systemVersion.Split('.');
        string version = versions[0] + "." + (versions.Length > 1 ? versions[1] : "");
        if (float.Parse(version) >= 14f) {
            _RequestIDFA(IDFAStatus);
        }
#endif
    }

}