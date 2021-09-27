using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Resources;

namespace Streamer.Bot {
    public class ImageManager : MonoBehaviour {
        public static ImageManager Instance;
        public Dictionary<string, Texture2D> ButtonImages = new Dictionary<string, Texture2D>();

        private void Start() {
            Instance = this;
        }

        private void OnDestroy() {
            Instance = null;
        }

        public void Clear() {
            ButtonImages.Clear();
        }

        public void RemoveButtonImage(string url) {
            if (ButtonImages.ContainsKey(url))
                ButtonImages.Remove(url);
        }

        public async Task GetDeckButtonImage(DeckButton button, string url) {
            Debug.Log("Getting Deck Button Image {url}");

            Texture2D tex = null;
            if (ButtonImages.ContainsKey(url)) {
                if (button) {
                    button.SetImage(ButtonImages[url]);

                    Debug.Log("Found Image already loaded {url}");
                    return;
                }
            }

            if ((tex = Resources.Load<Texture2D>(url)) != null) {
                ButtonImages.Add(url, tex);
                if (button) 
                    button.SetImage(tex);

                Debug.Log("Found Image in Resources {url}");
                return;
            }


            Debug.Log("Trying to download {url}");
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url)) {
                // begin request:
                var asyncOp = www.SendWebRequest();

                // await until it's done: 
                while (asyncOp.isDone == false)
                    await Task.Delay(500);

                // read results:
                if(www.result != UnityWebRequest.Result.Success ) {
                    // log error:
                    Debug.Log($"{www.error}, URL:{www.url}");
                } else {
                    // return valid results:
                    tex = DownloadHandlerTexture.GetContent(www);
                    if (tex != null) {
                        ButtonImages.Add(url, tex);
                    }

                    if (button)
                        button.SetImage(tex);
                    else {
                        Debug.LogError("Button is null!", gameObject);
                    }
                }
            }
        }
    }
}