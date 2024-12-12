using DG.Tweening;
using UnityEngine;

namespace FM.Template {
    public class CameraControl : MonoBehaviour {

        static CameraControl instance;

        public static Camera Camera;
        [SerializeField] Transform gimble;
        [SerializeField] Transform camT;

        void Awake() {
            instance = this;
            Camera = Camera.main;
        }

        public static void Shake() {
            Camera.DOShakePosition(0.3f, new Vector3(0.4f, 0.4f, 0.4f), 60, 90, true);
        }

    }
}