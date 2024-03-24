using System.Collections.Generic;
using UnityEngine;

namespace FM.Template {
    public class Sound : MonoBehaviour {

        public static bool soundOn;

        static Sound instance;
        Dictionary<string, AudioSource> sounds;

        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;

            sounds = new Dictionary<string, AudioSource>();

            AudioClip[] loadedSounds = Resources.LoadAll<AudioClip>("Sounds");
            foreach (AudioClip sound in loadedSounds) {
                GameObject go = new GameObject();
                go.name = sound.name;
                go.transform.SetParent(transform);
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = sound;
                sounds.Add(go.name, audioSource);
            }
        }

        public static void PlaySound(string soundName, bool taptic = true) {
            Debug.Log("Playing Sound: " + soundName);
            if (instance == null || soundName == null) {
                return;
            }
            if (soundOn) {
                if (!instance.sounds.ContainsKey(soundName)) {
                    Debug.LogError("Sound not found: " + soundName);
                    return;
                }
                instance.sounds[soundName].Play();
            }
            if (taptic) {
                Taptic.Light();
            }
        }
    }
}