using UnityEngine;

namespace FM.Template {
    public partial class K : MonoBehaviour {

        public static string ImagesDir = Application.persistentDataPath + "/Images/";

        public static partial class Tag {
            public static string Player = "Player";
        }

        public static partial class Prefs {
            public static string Level = "Level"; // PlayerPrefs
            public static string Sound = "Sound"; // PlayerPrefs
            public static string Taptic = "Taptic"; // PlayerPrefs
            public static string Images = "Images"; // FilePrefs
            public static string Coins = "Coins"; // FilePrefs
        }

        public static bool ColorCompare(Color32 color1, Color32 color2) {
            return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b;
        }

        public static Vector3 ParseVector3(string input) {
            string[] values = input.Split(',');
            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

    }
}