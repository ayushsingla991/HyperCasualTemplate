using UnityEngine;

public class TouchInput : MonoBehaviour {

    static TouchInput instance;

    bool isPressed;

    System.Action<bool> OnTouch;
    System.Action OnTouchHold;

    void Awake() {
        instance = this;
    }

    void Update() {
        if (GameManager.GetGameState() != GameState.Start) {
            return;
        }
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetKeyDown(KeyCode.Space))) {
            isPressed = true;
            if (OnTouch != null) {
                OnTouch(true);
            }
        }
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetKeyUp(KeyCode.Space))) {
            isPressed = false;
            if (OnTouch != null) {
                OnTouch(false);
            }
        }
        if (isPressed) {
            if (OnTouchHold != null) {
                OnTouchHold();
            }
        }
    }

    public static void AddOnTouch(System.Action<bool> _OnTouch) {
        instance.OnTouch += _OnTouch;
    }

    public static void AddOnTouchHold(System.Action _OnTouchHold) {
        instance.OnTouchHold += _OnTouchHold;
    }

}