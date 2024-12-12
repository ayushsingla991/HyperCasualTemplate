using DG.Tweening;
using FM.Template;
using UnityEngine;

namespace FM.Template {
    public class GoalPanel : MonoBehaviour {

        [SerializeField] RectTransform rect;
        [SerializeField] RectTransform goalTitle;

        void Start() {
            Timer.Delay(0.5f, () => {
                rect.anchoredPosition = new Vector2(0, -4000);
                rect.localScale = Vector3.one * 1.5f;
                rect.DOAnchorPosY(-1200f, 1.5f).SetEase(Ease.OutBack);
                Timer.Delay(2.5f, () => {
                    rect.DOAnchorPosY(-470f, 1f);
                    rect.DOScale(1f, 1f).SetEase(Ease.InOutBack);
                    goalTitle.DOAnchorPosY(0, 1f);
                });
            });
        }
    }
}