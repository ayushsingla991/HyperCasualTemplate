using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPopup : MonoBehaviour {

    static LoadingPopup instance;
    [SerializeField] GameObject popupPrefab;
    [SerializeField] Transform canvas;

    GameObject popupGO;

    GameObject previousSelected;

    void Awake() {
        instance = this;
    }

    public static void Hide() {
        if (instance.popupGO != null) {
            UIControls.Select(instance.previousSelected);
            Destroy(instance.popupGO);
            instance.popupGO = null;
        }
    }

    public static void Show(string loadingText) {
        Hide();
        instance.previousSelected = UIControls.Selected();
        RectTransform popupPanel = instance.ShowPopup(instance.popupPrefab);
        instance.popupGO = popupPanel.gameObject;
        popupPanel.GetComponent<Button>().onClick.AddListener(() => {
            Hide();
        });
        popupPanel.GetComponentInChildren<TextMeshProUGUI>().text = loadingText;
        UIControls.Select(popupPanel.GetComponent<Button>().gameObject);
    }

    RectTransform ShowPopup(GameObject prefab) {
        RectTransform popupPanel = Instantiate(prefab).GetComponent<RectTransform>();
        instance.popupGO = popupPanel.gameObject;
        popupPanel.SetParent(instance.canvas);
        popupPanel.localScale = new Vector3(1, 1, 1);
        popupPanel.offsetMin = new Vector2(0, 0);
        popupPanel.offsetMax = new Vector2(0, 0);
        return popupPanel;
    }

}