using TMPro;
using UnityEngine;

namespace FM.Template {
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
}