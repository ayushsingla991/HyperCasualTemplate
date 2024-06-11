using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class CursorHand : MonoBehaviour {

        [SerializeField] Canvas canvas;
        [SerializeField] RectTransform canvasRect;
        [SerializeField] CanvasScaler canvasScaler;
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
            Vector2 mousePos = Input.mousePosition;

            // Convert the screen coordinates to local coordinates of the canvas
            RectTransform canvasRectTransform = canvasRect;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePos, null, out localPoint);

            // Update the position of the finger UI element
            finger.localPosition = localPoint;

            // finger.anchoredPosition = UnscalePosition(Input.mousePosition);
        }

        Vector2 UnscalePosition(Vector2 vec) {
            Vector2 referenceResolution = canvasScaler.referenceResolution;
            Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

            float widthRatio = currentResolution.x / referenceResolution.x;
            float heightRatio = currentResolution.y / referenceResolution.y;

            float ratio = Mathf.Lerp(heightRatio, widthRatio, canvasScaler.matchWidthOrHeight);

            return vec / ratio;
        }

    }
}