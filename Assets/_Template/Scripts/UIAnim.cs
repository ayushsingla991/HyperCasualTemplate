using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class UIAnim : MonoBehaviour {

        [SerializeField] Image bg;
        [SerializeField] Transform popup;
        [SerializeField] Transform[] stars;
        [SerializeField] Transform continueButton;
        [SerializeField] GameState gameState;
        [SerializeField] float delay;

        void Start() {
            transform.localScale = Vector3.zero;
            bg.DOFade(0, 0);

            GameManager.AddOnGameStateChanged(_gameState => {

                if (_gameState == gameState) {
                    Timer.Delay(delay, () => {
                        bg.DOFade(0, 0);
                        popup.localScale = Vector3.zero;
                        foreach (Transform star in stars) {
                            star.localScale = Vector3.zero;
                        }
                        continueButton.localScale = Vector3.zero;
                        transform.localScale = Vector3.one;
                        bg.DOFade(0.8f, UIManager.ANIM_DUR);
                        popup.DOScale(Vector3.one, UIManager.ANIM_DUR).SetEase(Ease.OutBack).OnComplete(() => {
                            if (stars.Length > 0) {
                                StartCoroutine(StarScale(() => {
                                    continueButton.DOScale(Vector3.one, UIManager.ANIM_DUR / 2f).SetEase(Ease.OutBack);
                                }));
                            } else {
                                continueButton.DOScale(Vector3.one, UIManager.ANIM_DUR / 2f).SetEase(Ease.OutBack);
                            }
                        });
                    });
                } else {
                    transform.localScale = Vector3.zero;
                    bg.DOFade(0, 0);
                }
            });
        }

        IEnumerator StarScale(System.Action _OnComplete) {
            foreach (Transform star in stars) {
                star.DOScale(Vector3.one, UIManager.ANIM_DUR / 2f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(UIManager.ANIM_DUR / 2f);
            }
            if (_OnComplete != null) {
                _OnComplete();
            }
        }

    }
}