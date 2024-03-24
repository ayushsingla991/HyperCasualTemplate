using System.IO;
using UnityEngine;

namespace FM.Template {
    public class FilePrefs : MonoBehaviour {

        public static string GetString(string key, string defaultValue) {
            string filePath = Path.Combine(Application.persistentDataPath, key);
            Debug.Log(filePath);
            if (!File.Exists(filePath)) {
                return defaultValue;
            }
            using(StreamReader reader = new StreamReader(filePath)) {
                return reader.ReadToEnd();
            }
        }

        public static void SetString(string key, string value) {
            string filePath = Path.Combine(Application.persistentDataPath, key);
            if (!File.Exists(filePath)) {
                FileStream fileStream = File.Create(filePath);
                fileStream.Close();
            }
            using(StreamWriter writer = new StreamWriter(filePath)) {
                writer.Write(value);
            }
        }

    }
}