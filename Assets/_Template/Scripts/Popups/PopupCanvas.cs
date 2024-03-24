using UnityEngine;

namespace FM.Template {
    public class PopupCanvas : MonoBehaviour {

        static PopupCanvas instance;

        int overlayCount = 0;
        public static bool ShowingOverlay {
            set {
                instance.overlayCount = value ? instance.overlayCount + 1 : instance.overlayCount - 1;
                if (instance.overlayCount < 0) {
                    instance.overlayCount = 0;
                }
            }
            get {
                return instance.overlayCount > 0;
            }
        }

        void Awake() {
            instance = this;
        }
    }
}