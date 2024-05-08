using UnityEngine;

namespace FM.Template {
    public class TouchInput : MonoBehaviour {

        static TouchInput instance;

        bool isPressed;

        System.Action<bool> OnTouch;
        System.Action<bool, Vector2> OnTouchPos;
        System.Action OnTouchHold;
        System.Action<Vector2> OnDrag;

        Vector2 lastMousePos;

        void Awake() {
            instance = this;
        }

        void Update() {
            if (GameManager.GameState != GameState.Start) {
                return;
            }
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetKeyDown(KeyCode.Space))) {
                isPressed = true;
                lastMousePos = Input.mousePosition;
                OnTouchPos?.Invoke(true, lastMousePos);
                OnTouch?.Invoke(true);
            }
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetKeyUp(KeyCode.Space))) {
                isPressed = false;
                OnTouchPos?.Invoke(false, Vector2.zero);
                OnTouch?.Invoke(false);
            }
            if (isPressed) {
                Vector2 mousePos = Input.mousePosition;
                OnTouchHold?.Invoke();
                Vector2 diff = mousePos - lastMousePos;
                OnDrag?.Invoke(diff);
                lastMousePos = mousePos;
            }
        }

        public static void AddOnTouch(System.Action<bool> _OnTouch) {
            instance.OnTouch += _OnTouch;
        }

        public static void AddOnTouch(System.Action<bool, Vector2> _OnTouch) {
            instance.OnTouchPos += _OnTouch;
        }

        public static void AddOnTouchHold(System.Action _OnTouchHold) {
            instance.OnTouchHold += _OnTouchHold;
        }

        public static void AddOnDrag(System.Action<Vector2> _OnDrag) {
            instance.OnDrag += _OnDrag;
        }

    }
}