using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour {

    [SerializeField] Text attStatus;

    public void ShowPopup() {
        IDFA.RequestPopup(status => {
            Debug.Log(status);
            attStatus.text = status.ToString();
        });
    }

}