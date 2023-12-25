using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FM.Template {
    public enum Method { GET, POST }
    public class Web : MonoBehaviour {

        static Web instance;

        public static string BASE_URL = "https://fatmachines.com";
        public static string CONN = "https://google.com";

        static int MEDIA_TIMEOUT = 60;
        static int TIMEOUT = 30;

        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;

            UnityWebRequest.ClearCookieCache();
        }

        public static void IsConnected(System.Action<bool> _OnRequest) {
            Get(CONN, null, null, (_res, _err) => {
                _OnRequest(_err);
            });
        }

        public static void Image(string url, System.Action<Texture, bool> _OnRequest) {
            instance._Image(url, _OnRequest);
        }

        public static void Get(string url, Dictionary<string, object> headers, Dictionary<string, object> formData, System.Action<string, bool> _OnRequest) {
            instance._Get(url, headers, formData, _OnRequest);
        }

        public static void Post(string url, Dictionary<string, object> headers, Dictionary<string, object> formData, System.Action<string, bool> _OnRequest) {
            instance._Post(url, headers, formData, _OnRequest);
        }

        async void _Get(string url, Dictionary<string, object> headers, Dictionary<string, object> formData, System.Action<string, bool> _OnRequest) {
            if (formData != null && formData.Count > 0) {
                url += "?";
                foreach (KeyValuePair<string, object> param in formData) {
                    url += param.Key + "=" + param.Value + "&";
                }
                url = url.Remove(url.Length - 1, 1);
            }

            // Debug.Log("GET: " + url);

            UnityWebRequest www = UnityWebRequest.Get(url);
            www.timeout = TIMEOUT;

            if (headers == null) {
                headers = new Dictionary<string, object>();
            }
            headers.Add("accept", "application/json");
            headers.Add("Content-Type", "application/x-www-form-urlencoded");
            headers.Add("DeviceOS", SystemInfo.operatingSystem);

            if (headers != null) {
                foreach (KeyValuePair<string, object> header in headers) {
                    www.SetRequestHeader(header.Key, header.Value as string);
                }
            }

            // Debug.Log("Headers: " + new JsonFx.Json.JsonWriter().Write(headers));
            // Debug.Log("Form Data: " + new JsonFx.Json.JsonWriter().Write(formData));

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone) {
                await Task.Yield();
            }
            if (www.result == UnityWebRequest.Result.Success) {
                // Debug.Log(url + "\n" + www.downloadHandler.text);
                if (_OnRequest != null) {
                    _OnRequest(www.downloadHandler.text, false);
                }
            } else {
                Debug.LogError(url + "\n" + www.error);
                Debug.Log("Headers: " + headers == null ? "null" : Json.Serialize(headers));
                Debug.Log("Form Data: " + formData == null ? "null" : Json.Serialize(formData));
                if (_OnRequest != null) {
                    _OnRequest(www.downloadHandler.text, true);
                }
            }
            www.Dispose();
        }

        async void _Post(string url, Dictionary<string, object> headers, Dictionary<string, object> formData, System.Action<string, bool> _OnRequest) {
            WWWForm form = new WWWForm();
            if (formData != null) {
                foreach (KeyValuePair<string, object> entry in formData) {
                    form.AddField(entry.Key, entry.Value.ToString());
                }
            }

            // Debug.Log("POST: " + url);

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            www.timeout = TIMEOUT;

            if (headers == null) {
                headers = new Dictionary<string, object>();
            }
            headers.Add("accept", "application/json");
            headers.Add("Content-Type", "application/x-www-form-urlencoded");
            headers.Add("DeviceOS", SystemInfo.operatingSystem);

            if (headers != null) {
                foreach (KeyValuePair<string, object> header in headers) {
                    www.SetRequestHeader(header.Key, header.Value as string);
                }
            }

            // Debug.Log("Headers: " + Json.Serialize(headers));
            // Debug.Log("Form Data: " + Json.Serialize(formData));

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone) {
                await Task.Yield();
            }
            if (www.result == UnityWebRequest.Result.Success) {
                // Debug.Log(url + "\n" + www.downloadHandler.text);
                if (_OnRequest != null) {
                    _OnRequest(www.downloadHandler.text, false);
                }
            } else {
                Debug.LogError(url + "\n" + www.downloadHandler.text);
                Debug.Log("Headers: " + headers == null ? "null" : Json.Serialize(headers));
                Debug.Log("Form Data: " + formData == null ? "null" : Json.Serialize(formData));
                if (_OnRequest != null) {
                    _OnRequest(www.downloadHandler.text, true);
                }
            }
            www.Dispose();
        }

        async void _Image(string url, System.Action<Texture, bool> _OnRequest) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            www.timeout = MEDIA_TIMEOUT;
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone) {
                await Task.Yield();
            }
            if (www.result == UnityWebRequest.Result.Success) {
                if (_OnRequest != null) {
                    _OnRequest(DownloadHandlerTexture.GetContent(www), false);
                }
            } else {
                if (_OnRequest != null) {
                    _OnRequest(null, true);
                }
            }
            www.Dispose();
        }

    }
}