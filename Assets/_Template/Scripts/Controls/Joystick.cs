using UnityEngine;

namespace FM.Template {
    public class Joystick : MonoBehaviour {

        static Joystick instance;

        bool isPressed;
        Camera cam;
        Vector3 lastMousePos;
        Vector3 lastRotation;

        [SerializeField] Transform mover;
        Transform target;

        System.Action<Transform> OnDrag;

        bool aimLock;

        void Awake() {
            instance = this;
            Input.multiTouchEnabled = false;
        }

        void Start() {
            cam = Camera.main;
        }

        public static void OnSwerve(System.Action<Transform> _OnDrag) {
            instance.OnDrag = _OnDrag;
        }

        void Update() {
            if (target == null) {
                return;
            }
            transform.position = target.position;
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
                isPressed = true;
                Vector3 mPos = Input.mousePosition;
                mPos.z = 5;
                lastMousePos = cam.ScreenToViewportPoint(mPos);
                lastRotation = transform.eulerAngles;
            }
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) {
                isPressed = false;
                mover.localPosition = Vector3.zero;
            }
            if (isPressed) {
                if (!aimLock) {
                    Vector3 mPos = Input.mousePosition;
                    mPos.z = 5;
                    Vector3 mousePos = cam.ScreenToViewportPoint(mPos);
                    Vector3 diff = mousePos - lastMousePos;
                    diff.z = diff.y;
                    diff.y = 0;

                    mover.localPosition += diff;

                    lastMousePos = mousePos;
                    lastRotation = transform.eulerAngles;
                }
                if (OnDrag != null) {
                    OnDrag(mover);
                }
            }
        }

        public static void SetTarget(Transform _target) {
            instance.target = _target;
        }

    }

    // Joystick.SetTarget(transform);
    // Joystick.OnSwerve(_target => {
    //     if (GameManager.GetGameState() == GameState.Connecting || GameManager.GetGameState() == GameState.Start) {
    //         if (_target.position - transform.position != Vector3.zero) {
    //             Quaternion toRot = Quaternion.LookRotation(swerveDir == 1 ? _target.position - transform.position : transform.position - _target.position);
    //             transform.DORotate(new Vector3(0, toRot.eulerAngles.y, 0), 0.2f);
    //         }
    //     }
    // });
}