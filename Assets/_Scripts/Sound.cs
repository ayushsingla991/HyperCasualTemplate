using UnityEngine;

public class Sound : MonoBehaviour {

    public static bool soundOn;

    [SerializeField] AudioSource buttonClick;
    [SerializeField] AudioSource error;
    [SerializeField] AudioSource coin;
    [SerializeField] AudioSource win;

    static Sound instance;

    void Awake() {
        instance = this;
    }

    public static void PlayButton() {
        if (soundOn) {
            instance.buttonClick.Play();
        }
    }

    public static void Error() {
        if (soundOn) {
            instance.error.Play();
        }
    }

    public static void Coin() {
        if (soundOn) {
            instance.coin.Play();
        }
    }

    public static void Win() {
        if (soundOn) {
            instance.win.Play();
        }
    }
}