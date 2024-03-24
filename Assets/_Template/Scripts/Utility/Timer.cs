using System.Collections;
using UnityEngine;

namespace FM.Template {
    public class Timer : MonoBehaviour {

        static Timer instance;

        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        public static Coroutine Delay(float delay, System.Action OnComplete) {
            if (instance == null) {
                return null;
            }
            return instance.StartCoroutine(instance.CoDelay(delay, OnComplete));
        }

        public static Coroutine DelayFrame(System.Action OnComplete) {
            if (instance == null) {
                return null;
            }
            return instance.StartCoroutine(instance.CoDelayFrame(OnComplete));
        }

        IEnumerator CoDelay(float delay, System.Action OnComplete) {
            yield return new WaitForSeconds(delay);
            if (OnComplete != null) {
                OnComplete();
            }
        }

        IEnumerator CoDelayFrame(System.Action OnComplete) {
            yield return new WaitForEndOfFrame();
            if (OnComplete != null) {
                OnComplete();
            }
        }

    }
}