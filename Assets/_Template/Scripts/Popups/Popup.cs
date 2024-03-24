using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class Popup : MonoBehaviour {

        public bool IsShowing;
        bool IsAnimating;

        [SerializeField] Image bg;
        [SerializeField] bool animate = true;
        [SerializeField] RectTransform[] popups;
        [SerializeField] Transform[] animateItems;
        Vector3[] animItemsTargetScale;

        public virtual void Show(float bgFade = 0) {
            if (IsShowing) {
                return;
            }
            if (bg != null) {
                bg.gameObject.SetActive(true);
                bg.DOFade(0f, 0f);
                bg.DOFade(bgFade == 0 ? 0.7f : bgFade, UIManager.ANIM_DUR);
            }

            int animIndex = 0;
            animItemsTargetScale = new Vector3[animateItems.Length];
            foreach (Transform animateItem in animateItems) {
                animateItem.gameObject.SetActive(true);
                animItemsTargetScale[animIndex] = animateItem.localScale;
                animateItem.localScale = Vector3.zero;
                animIndex++;
            }

            if (popups != null) {
                IsAnimating = true;
                foreach (Transform popup in popups) {
                    popup.gameObject.SetActive(true);
                    popup.localScale = Vector3.zero;
                    popup.DOScale(Vector3.one, animate ? UIManager.ANIM_DUR : 0).SetEase(Ease.OutBack);
                }
                Timer.Delay(UIManager.ANIM_DUR, () => {
                    animIndex = 0;
                    foreach (Transform animateItem in animateItems) {
                        animateItem.DOScale(animItemsTargetScale[animIndex], animate ? UIManager.ANIM_DUR / 2f : 0.1f).SetEase(Ease.OutBack);
                        animIndex++;
                    }
                    if (animateItems.Length > 0) {
                        Sound.PlaySound("woosh");
                    }
                    IsAnimating = false;
                });
            }

            IsShowing = true;
            PopupCanvas.ShowingOverlay = true;
        }

        public virtual void Close() {
            if (!IsShowing) {
                return;
            }
            if (IsAnimating) {
                return;
            }
            IsAnimating = true;
            if (bg != null) {
                bg.DOFade(0f, UIManager.ANIM_DUR).OnComplete(() => {
                    bg.gameObject.SetActive(false);
                });
            }
            if (popups != null) {
                foreach (Transform popup in popups) {
                    popup.DOScale(Vector3.zero, animate ? UIManager.ANIM_DUR : 0).SetEase(Ease.InBack).OnComplete(() => {
                        popup.gameObject.SetActive(false);
                    });
                }
                Timer.Delay(UIManager.ANIM_DUR, () => {
                    IsAnimating = false;
                    IsShowing = false;
                    PopupCanvas.ShowingOverlay = false;
                });
            }
        }

    }
}