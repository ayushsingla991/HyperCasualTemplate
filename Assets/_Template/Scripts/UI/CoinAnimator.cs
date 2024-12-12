using UnityEngine;

namespace FM.Template {
    public class CoinAnimator : MonoBehaviour {

        static CoinAnimator instance;

        [SerializeField] GameObject coinAnimPrefab;
        [SerializeField] RectTransform canvas;

        void Awake() {
            instance = this;
        }

        public static void Animate(Transform origin, Transform destination, System.Action OnComplete = null) {
            CoinAnim anim = Instantiate(instance.coinAnimPrefab, origin.position, Quaternion.identity, instance.canvas).GetComponent<CoinAnim>();
            anim.origin = origin;
            anim.destination = destination;
            anim.OnComplete = OnComplete;
        }

    }
}