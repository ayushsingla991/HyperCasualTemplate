using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FM.Template {
    public class CoinUI : MonoBehaviour {

        [SerializeField] TextMeshProUGUI coinsText;

        void Start() {
            coinsText.text = "" + CoinsManager.Coins;
            CoinsManager.AddOnCoinsChanged(() => {
                coinsText.text = "" + CoinsManager.Coins;
                transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            });
        }
    }
}