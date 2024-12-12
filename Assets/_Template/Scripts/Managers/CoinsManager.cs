using FM.Template;
using TMPro;
using UnityEngine;

namespace FM.Template {
    public class CoinsManager : MonoBehaviour {

        static CoinsManager instance;

        int _coins;
        public static int Coins {
            get {
                return instance._coins;
            }
            set {
                int oldValue = instance._coins;
                instance._coins = value;
                if (oldValue != value) {
                    instance.OnCoinsChanged?.Invoke();
                }
            }
        }

        int _coinsToAdd;
        public static int CoinsToAdd {
            get {
                return instance._coinsToAdd;
            }
            set {
                instance._coinsToAdd = value;
            }
        }

        System.Action OnCoinsChanged;

        public static void AddOnCoinsChanged(System.Action _OnCoinsChanged) {
            instance.OnCoinsChanged += _OnCoinsChanged;
        }

        void Awake() {
            instance = this;

            Coins = PlayerPrefs.GetInt(K.Prefs.Coins, 0);
            CoinsToAdd = 0;
        }

        public static void Add(int amount) {
            Coins += amount;
            PlayerPrefs.SetInt(K.Prefs.Coins, Coins);
        }

        public static void Deduct(int amount) {
            Coins -= amount;
            PlayerPrefs.SetInt(K.Prefs.Coins, Coins);
        }
    }
}