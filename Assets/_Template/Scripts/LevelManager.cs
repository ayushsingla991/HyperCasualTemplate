using UnityEngine;

namespace FM.Template {
    public class LevelManager : MonoBehaviour {

        static LevelManager instance;

        [SerializeField] int level;
        public static int Level {
            get {
                return instance.level;
            }
        }
        int levelIndex;

        void Awake() {
            instance = this;

            if (level == -1) {
                level = PlayerPrefs.GetInt(K.Prefs.Level, 1);
            }
        }

        void Start() {
            GameManager.AddOnGameStateChanged(_gameState => {
                if (_gameState == GameState.Win) {
                    PlayerPrefs.SetInt(K.Prefs.Level, level + 1);
                    PlayerPrefs.Save();
                }
            });
        }

    }
}