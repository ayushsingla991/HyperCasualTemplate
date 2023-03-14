using UnityEngine;

public class LevelManager : MonoBehaviour {

    static LevelManager instance;

    [SerializeField] int level;
    int levelIndex;

    void Awake() {
        instance = this;

        if (level == -1) {
            level = PlayerPrefs.GetInt(K.Prefs.Level, 0);
        }
    }

    void Start() {
        GameManager.AddOnGameStateChanged(_gameState => {
            if (_gameState == GameState.Win) {
                PlayerPrefs.SetInt(K.Prefs.Level, level + 1);
            }
        });
    }

    public static int Level() {
        return instance.level;
    }

}