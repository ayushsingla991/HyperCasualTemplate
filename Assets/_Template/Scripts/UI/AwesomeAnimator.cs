using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace FM.Template {
    public class AwesomeAnimator : MonoBehaviour {

        static AwesomeAnimator instance;

        [SerializedDictionary("AlphabetName", "Alphabet")]
        public SerializedDictionary<string, Sprite> alphabets;

        [SerializeField] GameObject animPrefab;
        [SerializeField] RectTransform canvas;

        void Awake() {
            instance = this;

            Sprite[] caps = Resources.LoadAll<Sprite>("Alphabets/Caps");
            Sprite[] small = Resources.LoadAll<Sprite>("Alphabets/Small");
            Sprite[] numbers = Resources.LoadAll<Sprite>("Alphabets/Numbers");

            foreach (var alphabet in caps) {
                alphabets.Add(alphabet.name, alphabet);
            }

            foreach (var alphabet in small) {
                alphabets.Add(alphabet.name, alphabet);
            }

            foreach (var alphabet in numbers) {
                alphabets.Add(alphabet.name, alphabet);
            }
        }

        public static void Animate(string text, string text2 = "", float scale = 0.5f, float gapX = 300, float arcHeight = 250f, System.Action _OnComplete = null) {
            AwesomeAnim anim = Instantiate(instance.animPrefab, Vector3.zero, Quaternion.identity, instance.canvas).GetComponent<AwesomeAnim>();
            RectTransform rect = anim.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = new Vector3(scale, scale, 1);
            List<Sprite> sprites = new List<Sprite>();
            foreach (char c in text) {
                if (instance.alphabets.ContainsKey(c.ToString())) {
                    sprites.Add(instance.alphabets[c.ToString()]);
                }
            }

            List<Sprite> sprites2 = new List<Sprite>();
            foreach (char c in text2) {
                if (instance.alphabets.ContainsKey(c.ToString())) {
                    sprites2.Add(instance.alphabets[c.ToString()]);
                }
            }
            anim.Animate(sprites, sprites2, gapX, arcHeight, _OnComplete);
        }

    }
}