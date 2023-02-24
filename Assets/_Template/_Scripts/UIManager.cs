using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    static UIManager instance;

    public static float ANIM_DUR = 0.4f;

    [SerializeField] TextMeshProUGUI levelText;

    void Awake() {
        instance = this;
    }

    void Start() {
        levelText.text = "LEVEL " + (LevelManager.Level() + 1);
    }

}