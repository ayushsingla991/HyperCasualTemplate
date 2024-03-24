using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FM.Template {
    public class LosePopup : Popup {

        static LosePopup instance;

        [SerializeField] TextMeshProUGUI coinsText;

        void Awake() {
            instance = this;
        }

        void Start() {
            GameManager.AddOnGameStateChanged(_gameState => {
                if (_gameState == GameState.Lose) {
                    ShowPopup();
                }
            });
        }

        public static void ShowPopup(int coins = 0) {
            instance.Show();
            instance.coinsText.text = "" + coins;
        }
    }
}