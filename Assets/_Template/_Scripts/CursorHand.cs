using DG.Tweening;
using UnityEngine;

public class CursorHand : MonoBehaviour {

    [SerializeField] RectTransform finger;
    Tween scaler;

    void Start() {
        finger.gameObject.SetActive(true);

        TouchInput.AddOnTouch(_touch => {
            if (scaler != null) {
                scaler.Kill();
                scaler = null;
            }
            if (_touch) {
                scaler = finger.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.1f);
            } else {
                scaler = finger.DOScale(Vector3.one, 0.1f);
            }
        });
    }

    void Update() {
        finger.anchoredPosition = Input.mousePosition;
    }

}