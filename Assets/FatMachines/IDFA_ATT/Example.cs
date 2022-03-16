using UnityEngine;
using UnityEngine.UI;

namespace IDFAPlugin {
    public class Example : MonoBehaviour {

        [SerializeField] Text attStatus;

        private void Awake() {
            IDFA.idfaDesc = "This identifier will be used to deliver personalized ads to you.";
            IDFA.sdkAdIds.Add("n38lu8286q.skadnetwork");
            IDFA.sdkAdIds.Add("v9wttpbfk9.skadnetwork");
            IDFA.sdkAdIds.Add("cstr6suwn9.skadnetwork");
        }

        public void ShowPopup() {
            IDFA.RequestPopup(status => {
                Debug.Log(status);
                attStatus.text = status.ToString();
            });
        }

    }
}