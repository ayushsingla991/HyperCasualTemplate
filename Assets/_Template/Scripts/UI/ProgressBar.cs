using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class ProgressBar : MonoBehaviour {

        public float minValue = 0f;
        public float maxValue = 1f;
        float _progress = 0f;
        public float progress {
            get {
                return _progress;
            }
            set {
                _progress = value;
                fill.fillAmount = _progress / (maxValue - minValue);
            }
        }

        public void DOProgress(float value) {
            _progress = value;
            fill.DOFillAmount(value / (maxValue - minValue), 0.1f);
        }

        string _text;
        public string text {
            get {
                return _text;
            }
            set {
                _text = value;
                progressText.text = _text;
            }
        }

        [SerializeField] Image fill;
        [SerializeField] TextMeshProUGUI progressText;

    }
}