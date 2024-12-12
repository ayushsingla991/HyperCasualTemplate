using UnityEngine;

namespace FM.Template {
    public class Rotate : MonoBehaviour {

        [SerializeField] float speed;

        void Update() {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}