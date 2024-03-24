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
        }

    }
}