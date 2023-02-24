using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Idle, Tutorial, Start, Win, Lose }
public class GameManager : MonoBehaviour {

    static GameManager instance;

    GameState _gameState;
    GameState gameState {
        get {
            return _gameState;
        }
        set {
            if (_gameState != value) {
                if (OnGameStateChanged != null) {
                    OnGameStateChanged(value);
                }
            }
            _gameState = value;
        }
    }

    System.Action<GameState> OnGameStateChanged;

    [SerializeField] GameObject confettiPrefab;
    [SerializeField] Transform confettiPoint;

    void Awake() {
        instance = this;
        Application.targetFrameRate = 60;
        Sound.soundOn = true;
        Taptic.tapticOn = true;
    }

    void Start() {
        gameState = GameState.Idle;
        IDFA.RequestPopup();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Restart();
        }
    }

    public void StartTutorial() {
        gameState = GameState.Start;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void GameOver() {
        instance.gameState = GameState.Lose;
        Taptic.Failure();
    }

    public static void GameWon() {
        Sound.Win();
        Taptic.Success();
        instance.gameState = GameState.Win;
        Instantiate(instance.confettiPrefab, instance.confettiPoint.position, instance.confettiPoint.rotation, instance.confettiPoint);
    }

    public static GameState GetGameState() {
        return instance.gameState;
    }

    public static void AddOnGameStateChanged(System.Action<GameState> _OnGameStateChanged) {
        instance.OnGameStateChanged += _OnGameStateChanged;
    }

    public static void RemoveOnGameStateListener(System.Action<GameState> _OnGameStateChanged) {
        instance.OnGameStateChanged -= _OnGameStateChanged;
    }

}