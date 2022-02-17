using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour {

    static Timer instance;

    void Awake() {
        instance = this;
    }

    public static Coroutine Delay(float delay, System.Action OnComplete) {
        return instance.StartCoroutine(instance.CoDelay(delay, OnComplete));
    }

    IEnumerator CoDelay(float delay, System.Action OnComplete) {
        yield return new WaitForSeconds(delay);
        if (OnComplete != null) {
            OnComplete();
        }
    }

}