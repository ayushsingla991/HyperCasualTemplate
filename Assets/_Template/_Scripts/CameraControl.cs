using DG.Tweening;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    static CameraControl instance;

    Camera cam;
    [SerializeField] Transform gimble;
    [SerializeField] Transform camT;

    void Awake() {
        instance = this;
        cam = Camera.main;
    }

    public static void Shake() {
        instance.cam.DOShakePosition(0.3f, new Vector3(0.4f, 0.4f, 0.4f), 60, 90, true);
    }

}