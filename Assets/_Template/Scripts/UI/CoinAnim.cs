using DG.Tweening;
using FM.Template;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FM.Template {
    public class CoinAnim : MonoBehaviour {

        public Transform origin;
        public Transform destination;

        List<Vector3> originPositions;

        public System.Action OnComplete;

        void Start() {
            Sound.PlaySound("coin_collect");
            originPositions = new List<Vector3>();
            for (int i = 0; i < transform.childCount; i++) {
                originPositions.Add(transform.GetChild(i).localPosition);
                transform.GetChild(i).localPosition = Vector3.zero;
                transform.GetChild(i).localScale = origin.localScale;
                transform.GetChild(i).DOLocalMove(originPositions[i], 0.25f).SetEase(Ease.OutBack);
            }
            StartCoroutine(Animate());
        }

        IEnumerator Animate() {
            yield return new WaitForSeconds(0.25f);
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).DOScale(destination.localScale, 0.4f);
                transform.GetChild(i).DOMove(destination.position, 0.4f).OnComplete(() => {
                    Taptic.Light();
                });
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.3f);
            OnComplete?.Invoke();
            Destroy(gameObject);
        }

    }
}