using UnityEngine;
using UnityEngine.EventSystems;

public class UIControls : MonoBehaviour {

    static UIControls instance;

    [SerializeField] GameObject playButton, restartButton, nextLevelButton;

    void Awake() {
        instance = this;
    }

    void Start() {
        EventSystem.current.SetSelectedGameObject(null);
        if (!Mobile()) {
            EventSystem.current.SetSelectedGameObject(playButton);
        }
        GameManager.AddOnGameStateChanged(_gameState => {
            EventSystem.current.SetSelectedGameObject(null);
            if (!Mobile()) {
                InitSelection(_gameState);
            }
        });
    }

    void InitSelection(GameState _gameState) {
        if (_gameState == GameState.Idle) {
            EventSystem.current.SetSelectedGameObject(playButton);
        } else if (_gameState == GameState.Lose) {
            EventSystem.current.SetSelectedGameObject(restartButton);
        } else if (_gameState == GameState.Win) {
            EventSystem.current.SetSelectedGameObject(nextLevelButton);
        }
    }

    void Update() {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            if (EventSystem.current.currentSelectedGameObject == null) {
                InitSelection(GameManager.GetGameState());
            }
        }
    }

    bool Mobile() {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public static void Select(GameObject _go) {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_go);
    }

    public static GameObject Selected() {
        return EventSystem.current.currentSelectedGameObject;
    }

}