using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GDPRPopup : MonoBehaviour {

    static GDPRPopup instance;
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

    public static void Show(System.Action<bool> gdprDone) {
        Hide();
        instance.previousSelected = UIControls.Selected();
        RectTransform popupPanel = instance.ShowPopup(instance.popupPrefab);
        instance.popupGO = popupPanel.gameObject;
        popupPanel.GetChild(0).Find("PositiveButton").GetComponent<Button>().onClick.AddListener(() => {
            gdprDone(true);
            Hide();
        });
        popupPanel.GetChild(0).Find("NegativeButton").GetComponent<Button>().onClick.AddListener(() => {
            gdprDone(false);
            Hide();
        });
        UIControls.Select(popupPanel.GetChild(0).Find("PositiveButton").gameObject);
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