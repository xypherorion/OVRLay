using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Resources;
using System.Linq;

namespace Streamer.Bot {
    public class ImageManager : MonoBehaviour {
        public static ImageManager _instance = null;
        public static ImageManager instance {
            get {
                return _instance == null ? (_instance = FindObjectOfType<ImageManager>()) : _instance;
            }
        }

        public Dictionary<string, Texture2D> ButtonImages = new Dictionary<string, Texture2D>();

        private void Start() {
        }

        private void OnDestroy() {
        }

        public void Clear() {
            ButtonImages.Clear();
        }

        public void RemoveButtonImage(string url) {
            if (ButtonImages.ContainsKey(url))
                ButtonImages.Remove(url);
        }


        public Dictionary<string, UnityWebRequest> requestQueue = new Dictionary<string, UnityWebRequest>();

        public async Task GetDeckButtonImage(DeckButton button, string url) {
            Debug.Log($"Getting Deck Button Image {button.name} {url}");

            Texture2D tex = null;
            UnityWebRequest www = null;
            if (!requestQueue.ContainsKey(url)) {
                if (ButtonImages.ContainsKey(url)) {
                    tex = ButtonImages[url];
                    Debug.Log($"Found Image in Loaded Assets {url}");
                }

                if (tex == null) {
                    tex = Resources.Load<Texture2D>(url);
                    Debug.Log($"Found Image in Resources {url}");
                    ButtonImages.Add(url, tex);
                }

                Debug.Log($"Attempting download {url}");
                requestQueue.Add(url, www = UnityWebRequestTexture.GetTexture(url));
                    
                // begin request:
                var asyncOp = www.SendWebRequest();

                // await until it's done: 
                while (asyncOp.isDone == false)
                    await Task.Delay(500);
            } else {
                Debug.Log($"Awaiting download {url}");
                www = requestQueue[url];
                while(!www.isDone)
                    await Task.Delay(500);
            }

            if (www != null) {
                // read results:
                if (www.result != UnityWebRequest.Result.Success) {
                    // log error:
                    Debug.Log($"{www.error}, URL:{www.url}");
                } else {
                    // return valid results:
                    tex = DownloadHandlerTexture.GetContent(www);
                    if (tex != null) {
                        //Sometimes there's already a copy here
                        if (ButtonImages.ContainsKey(url))
                            tex = ButtonImages[url];
                        else
                            ButtonImages.Add(url, tex);
                    }

                    if (button)
                        button.SetImage(tex);
                    else {
                        Debug.LogError("Button is null!", gameObject);
                    }
                }
            }

            if (button)
                button.SetImage(tex);
        }
    }
}