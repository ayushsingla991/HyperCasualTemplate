using UnityEngine;
using UnityEngine.UI;

namespace FM.Template {
    public class Settings : MonoBehaviour {

        [SerializeField] GameObject settingsPanel;

        [SerializeField] Image soundButton;
        [SerializeField] Image tapticButton;

        [SerializeField] Sprite soundOn, soundOff;
        [SerializeField] Sprite tapticOn, tapticOff;

        void Start() {
            settingsPanel.SetActive(false);
            Sound.soundOn = PlayerPrefs.GetInt(K.Prefs.Sound, 1) == 1 ? true : false;
            Taptic.tapticOn = PlayerPrefs.GetInt(K.Prefs.Taptic, 1) == 1 ? true : false;

            soundButton.sprite = Sound.soundOn ? soundOn : soundOff;
            tapticButton.sprite = Taptic.tapticOn ? tapticOn : tapticOff;
        }

        public void ToggleSound() {
            Sound.soundOn = !Sound.soundOn;
            soundButton.sprite = Sound.soundOn ? soundOn : soundOff;
            PlayerPrefs.SetInt(K.Prefs.Sound, Sound.soundOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ToggleTaptic() {
            Taptic.tapticOn = !Taptic.tapticOn;
            tapticButton.sprite = Taptic.tapticOn ? tapticOn : tapticOff;
            PlayerPrefs.SetInt(K.Prefs.Taptic, Taptic.tapticOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void Show() {
            settingsPanel.SetActive(true);
        }

        public void Close() {
            settingsPanel.SetActive(false);
        }

    }
}