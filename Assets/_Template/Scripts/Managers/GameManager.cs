using UnityEngine;
using UnityEngine.SceneManagement;

namespace FM.Template {
    public enum GameState { Idle, Tutorial, Start, Win, Lose }
    public class GameManager : MonoBehaviour {

        static GameManager instance;

        GameState _gameState;
        public static GameState GameState {
            get {
                return instance._gameState;
            }
            private set {
                if (instance._gameState != value) {
                    if (instance.OnGameStateChanged != null) {
                        instance.OnGameStateChanged(value);
                    }
                }
                instance._gameState = value;
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
            GameState = GameState.Idle;
        }

        public static void Restart() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void StartGame() {
            GameState = GameState.Start;
        }

        public static void GameOver() {
            GameState = GameState.Lose;
            Taptic.Failure();
        }

        public static void GameWon() {
            Sound.PlaySound("win");
            Taptic.Success();
            GameState = GameState.Win;
            Instantiate(instance.confettiPrefab, instance.confettiPoint.position, instance.confettiPoint.rotation, instance.confettiPoint);
        }

        public static void AddOnGameStateChanged(System.Action<GameState> _OnGameStateChanged) {
            instance.OnGameStateChanged += _OnGameStateChanged;
        }

        public static void RemoveOnGameStateListener(System.Action<GameState> _OnGameStateChanged) {
            instance.OnGameStateChanged -= _OnGameStateChanged;
        }

    }
}