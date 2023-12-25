using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FM.Template {
    public class Images : MonoBehaviour {

        static Images instance;
        Dictionary<string, object> imagesDB;

        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;

            imagesDB = Json.Deserialize(PlayerPrefs.GetString(K.Prefs.Images, "{}"))as Dictionary<string, object>;
        }

        public static void SaveImage(string url, Texture img) {
            Texture2D tex = (Texture2D)img;
            byte[] bytes = tex.EncodeToPNG();
            if (!Directory.Exists(K.ImagesDir)) {
                Directory.CreateDirectory(K.ImagesDir);
            }

            string[] splitFileName = url.Split('/');
            string fullFileName = splitFileName[splitFileName.Length - 1];

            string imageName = fullFileName.Split('.')[0];

            string filePath = K.ImagesDir + imageName + ".png";

            File.WriteAllBytes(filePath, bytes);

            if (instance.imagesDB.ContainsKey(url)) {
                instance.imagesDB[url] = filePath;
            } else {
                instance.imagesDB.Add(url, filePath);
            }
            PlayerPrefs.SetString(K.Prefs.Images, Json.Serialize(instance.imagesDB));
        }

        public static Texture2D GetImage(string url) {
            if (!instance.imagesDB.ContainsKey(url)) {
                return null;
            }
            Texture2D tex = null;
            byte[] fileData;

            string filePath = instance.imagesDB[url] as string;

            if (File.Exists(filePath)) {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }

        public static void GetSprite(string url, System.Action<Sprite> _callback) {
            Texture2D tex = GetImage(url);
            if (tex != null) {
                _callback?.Invoke(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero));
                return;
            }
            Web.Image(url, (_tex, _err) => {
                if (!_err) {
                    SaveImage(url, _tex);
                    GetSprite(url, _callback);
                } else {
                    _callback?.Invoke(null);
                }
            });
        }

        public static Sprite GetSprite(Texture _tex) {
            Texture2D tex = (Texture2D)_tex;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }

    }
}