using UnityEngine;

namespace FM.Template {
    public class K : MonoBehaviour {

        public static string ImagesDir = Application.persistentDataPath + "/Images/";

        public static class Tag {
            public static string Player = "Player";
        }

        public static class Prefs {
            public static string Level = "Level"; // PlayerPrefs
            public static string Sound = "Sound"; // PlayerPrefs
            public static string Taptic = "Taptic"; // PlayerPrefs
            public static string Images = "Images"; // PlayerPrefs
        }

    }
}