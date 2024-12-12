using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class AwesomeAnim : MonoBehaviour {

        System.Action OnComplete;

        [SerializeField] RectTransform sunrays;

        List<RectTransform> items;

        public void Animate(List<Sprite> sprites, List<Sprite> sprites2, float gapX, float arcHeight, System.Action _OnComplete) {

            OnComplete = _OnComplete;

            items = new List<RectTransform>();

            float startX = -gapX * (sprites.Count - 1) / 2f;

            int index = 0;

            float chordLength = gapX * (sprites.Count - 1); // Total horizontal distance between first and last items
            float radius = (chordLength * chordLength) / (8 * arcHeight) + arcHeight; // Circle radius
            float totalAngle = Mathf.Asin(chordLength / (2 * radius)) * 2 * Mathf.Rad2Deg; // Total angle in degrees
            float startAngle = -totalAngle / 2f; // Start angle for the first sprite

            foreach (var sprite in sprites) {
                GameObject alphabet = new GameObject();
                alphabet.name = sprite.name;
                alphabet.transform.SetParent(transform);
                alphabet.transform.localScale = Vector3.one;
                Image image = alphabet.AddComponent<Image>();
                image.sprite = sprite;
                image.SetNativeSize();
                RectTransform rect = alphabet.GetComponent<RectTransform>();

                float angle = startAngle + (index * (totalAngle / (sprites.Count - 1)));
                float x = startX + gapX * index;
                float y = -Mathf.Pow(x / (sprites.Count * gapX / 2f), 2) * arcHeight + arcHeight;
                rect.anchoredPosition = new Vector3(x, y, 0);
                rect.localRotation = Quaternion.Euler(0, 0, -angle);

                // rect.anchoredPosition = new Vector3(startX + gapX * index, 0, 0);
                index++;
                items.Add(rect);

                rect.localScale = Vector3.zero;
            }

            startX = -gapX * (sprites2.Count - 1) / 2f;
            index = 0;

            foreach (var sprite in sprites2) {
                GameObject alphabet = new GameObject();
                alphabet.name = sprite.name;
                alphabet.transform.SetParent(transform);
                alphabet.transform.localScale = Vector3.one;
                Image image = alphabet.AddComponent<Image>();
                image.sprite = sprite;
                image.SetNativeSize();
                RectTransform rect = alphabet.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector3(startX + gapX * index, -250, 0);
                index++;
                items.Add(rect);

                rect.localScale = Vector3.zero;
            }

            StartCoroutine(AnimateItems());

        }

        IEnumerator AnimateItems() {
            sunrays.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack);
            foreach (RectTransform item in items) {
                item.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(0.075f);
            }
            yield return new WaitForSeconds(1f);
            transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => {
                OnComplete?.Invoke();
                Destroy(gameObject);
            });
        }

    }
}