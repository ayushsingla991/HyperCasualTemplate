using UnityEngine;

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

    void Awake() {
        instance = this;
        Application.targetFrameRate = 60;
    }

    void Start() {
        gameState = GameState.Idle;
    }

    public void StartGame() {
        gameState = GameState.Tutorial;
    }

    public static GameState GetGameState() {
        return instance.gameState;
    }

    public static void AddOnGameStateChanged(System.Action<GameState> _OnGameStateChanged) {
        instance.OnGameStateChanged += _OnGameStateChanged;
    }

}