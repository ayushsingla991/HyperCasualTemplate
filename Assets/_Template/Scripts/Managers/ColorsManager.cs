using AYellowpaper.SerializedCollections;
using FM.Template;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FM.Template {
    public class ColorsManager : MonoBehaviour {

        static ColorsManager instance;

        [SerializedDictionary("Color Name", "Color")]
        public SerializedDictionary<string, Color> _blockColors;
        public static Dictionary<string, Color> BlockColors {
            get {
                return instance._blockColors;
            }
            private set {
                instance._blockColors = new SerializedDictionary<string, Color>();
                foreach (var color in value) {
                    instance._blockColors.Add(color.Key, color.Value);
                }
            }
        }

        [SerializedDictionary("Color", "Color Name")]
        public SerializedDictionary<Color, string> _blockColorsName;
        public static Dictionary<Color, string> BlockColorsName {
            get {
                return instance._blockColorsName;
            }
            private set {
                instance._blockColorsName = new SerializedDictionary<Color, string>();
                foreach (var color in value) {
                    instance._blockColorsName.Add(color.Key, color.Value);
                }
            }
        }

        void Awake() {
            instance = this;
            string colorStr = Resources.Load<TextAsset>("Colors").text;
            BlockColors = (Json.Deserialize(colorStr)as Dictionary<string, object>).ToDictionary(x => x.Key, x => HexToColor(x.Value as string));
            BlockColorsName = BlockColors.ToDictionary(x => x.Value, x => x.Key);
        }

        public static Color HexToColor(string colorString) {
            Color color;
            if (ColorUtility.TryParseHtmlString("#" + colorString, out color)) {
                return color;
            } else {
                Debug.LogWarning("Failed to parse color: " + colorString);
                return Color.white;
            }
        }
    }
}