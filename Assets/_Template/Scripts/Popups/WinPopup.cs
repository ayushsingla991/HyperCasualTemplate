using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FM.Template {
    public class WinPopup : Popup {

        static WinPopup instance;

        [SerializeField] Transform coinImg;
        [SerializeField] Transform coinHeaderImg;
        [SerializeField] TextMeshProUGUI coinsText;
        [SerializeField] TextMeshProUGUI completedText;

        [SerializeField] float delay;

        void Awake() {
            instance = this;
        }

        void Start() {
            GameManager.AddOnGameStateChanged(_gameState => {
                if (_gameState == GameState.Win) {
                    Timer.Delay(delay, () => {
                        ShowPopup(CoinsManager.CoinsToAdd);
                    });
                }
            });
        }

        public static void ShowPopup(int coins = 10) {
            instance.Show();
            instance.completedText.text = "LEVEL " + LevelManager.Level + "\nCOMPLETED!";
            instance.coinsText.text = "" + coins;
        }

        public void Continue() {
            if (CoinsManager.CoinsToAdd > 0) {
                CoinAnimator.Animate(coinImg, coinHeaderImg, () => {
                    CoinsManager.Add(CoinsManager.CoinsToAdd);
                    Timer.Delay(0.5f, () => {
                        GameManager.Restart();
                    });
                });
            }
        }

    }
}