using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FM.Template {
    public class WinPopup : Popup {

        static WinPopup instance;

        [SerializeField] Transform[] stars;
        [SerializeField] TextMeshProUGUI coinsText;

        void Awake() {
            instance = this;
        }

        void Start() {
            GameManager.AddOnGameStateChanged(_gameState => {
                if (_gameState == GameState.Win) {
                    ShowPopup();
                }
            });
        }

        IEnumerator StarScale(int starsCount, System.Action _OnComplete = null) {
            int count = 0;
            foreach (Transform star in stars) {
                star.DOScale(Vector3.one, UIManager.ANIM_DUR / 2f).SetEase(Ease.OutBack);
                count++;
                if (count == starsCount) {
                    break;
                }
                yield return new WaitForSeconds(UIManager.ANIM_DUR / 2f);
            }
            _OnComplete?.Invoke();
        }

        public static void ShowPopup(int stars = 3, int coins = 10) {
            instance.Show();
            foreach (Transform star in instance.stars) {
                star.localScale = Vector3.zero;
            }
            if (instance.stars.Length > 0) {
                instance.StartCoroutine(instance.StarScale(stars));
            }
            instance.coinsText.text = "" + coins;
        }
    }
}