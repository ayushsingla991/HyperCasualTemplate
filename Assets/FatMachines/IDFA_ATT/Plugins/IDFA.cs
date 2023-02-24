using UnityEngine;
#if UNITY_IOS
using AOT;
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

public class IDFA : MonoBehaviour {

    public enum Status { NOT_SUPPORTED, DENIED, AUTHORIZED, NOT_DETERMINED, RESTRICTED }

    static IDFA instance;

#if UNITY_IOS
    private delegate void DelegateIDFAStatus(int status);

    [DllImport("__Internal")]
    private static extern void _RequestIDFA(DelegateIDFAStatus callback);

    [MonoPInvokeCallback(typeof(DelegateIDFAStatus))]
    private static void IDFAStatus(int status) {
        Debug.Log("IDFA Status: " + status);
        if (OnIDFAStatus != null) {
            if (status == -1) {
                OnIDFAStatus(Status.NOT_SUPPORTED);
            } else if (status == 0) {
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

    static System.Action<Status> OnIDFAStatus;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public static void RequestPopup(System.Action<Status> _OnIDFAStatus = null) {
        OnIDFAStatus = _OnIDFAStatus;
#if UNITY_IOS
        if (Application.isEditor) {
            if (OnIDFAStatus != null) {
                OnIDFAStatus(Status.AUTHORIZED);
            }
            return;
        }
        _RequestIDFA(IDFAStatus);
#endif
    }

}