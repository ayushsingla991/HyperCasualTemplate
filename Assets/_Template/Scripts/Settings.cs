using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class Settings : MonoBehaviour {

        [SerializeField] Popup settingsPopup;

        [SerializeField] Image soundButton;
        [SerializeField] Image tapticButton;

        [SerializeField] Sprite switchOn, switchOff;

        void Start() {
            Sound.soundOn = PlayerPrefs.GetInt(K.Prefs.Sound, 1) == 1 ? true : false;
            Taptic.tapticOn = PlayerPrefs.GetInt(K.Prefs.Taptic, 1) == 1 ? true : false;

            soundButton.sprite = Sound.soundOn ? switchOn : switchOff;
            tapticButton.sprite = Taptic.tapticOn ? switchOn : switchOff;
        }

        public void ToggleSound() {
            Sound.soundOn = !Sound.soundOn;
            soundButton.sprite = Sound.soundOn ? switchOn : switchOff;
            PlayerPrefs.SetInt(K.Prefs.Sound, Sound.soundOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ToggleTaptic() {
            Taptic.tapticOn = !Taptic.tapticOn;
            tapticButton.sprite = Taptic.tapticOn ? switchOn : switchOff;
            PlayerPrefs.SetInt(K.Prefs.Taptic, Taptic.tapticOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void Show() {
            settingsPopup.Show();
        }

        public void Close() {
            settingsPopup.Close();
        }

    }
}