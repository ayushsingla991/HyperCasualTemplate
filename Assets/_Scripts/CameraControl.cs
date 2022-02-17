using UnityEngine;

public class CameraControl : MonoBehaviour{

    static CameraControl instnace;

    Camera cam;

    void Awake(){
        instnace = this;
        cam = Camera.main;
    }

}
