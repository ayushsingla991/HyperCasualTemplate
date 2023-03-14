using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour {

    static InfoPopup instance;
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

    public static void Show(string text, string buttonText) {
        Hide();
        LoadingPopup.Hide();
        instance.previousSelected = UIControls.Selected();
        RectTransform popupPanel = instance.ShowPopup(instance.popupPrefab);
        instance.popupGO = popupPanel.gameObject;
        popupPanel.GetComponent<Button>().onClick.AddListener(() => {
            Hide();
        });
        popupPanel.GetChild(0).Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        popupPanel.GetChild(0).Find("Button").Find("Text").GetComponent<TextMeshProUGUI>().text = buttonText;
        popupPanel.GetChild(0).Find("Button").GetComponent<Button>().onClick.AddListener(() => {
            Hide();
        });
        UIControls.Select(popupPanel.GetChild(0).Find("Button").gameObject);
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