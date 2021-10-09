using Streamer.Bot.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;

namespace Streamer.Bot {
    public class DeckButton : MonoBehaviour {
        public Data.DeckButton data;

        public RawImage image;
        public Button uiButton;
        public Text text;


        public void SetImage(Texture2D tex) {
            image.texture = tex;
        }

        public void SetData(Data.DeckButton data) {
            this.data = data;
        }

        public void ExecuteAction() {
            if (!string.IsNullOrWhiteSpace(data.action_id)) {
                WebSocketClient webClient = FindObjectOfType<WebSocketClient>();
                if(webClient) {
                    webClient.SendCommand(data.action_id, data.name, data.action_args?.ToString());
                }
            } else {
                Debug.LogError("Null or Empty Action");
            }
        }

        private async void Start() {
            //Check if data is valid by checkind creation timestamp
            if (data.created_at != default) {
                name = data.name;
                if (text)
                    text.text = name;
                if (image && data.image_url != null) {
                    if (data.image_url != null) {
                        //When this returns, it should set the image.texture for the button
                        await ImageManager.instance.GetDeckButtonImage(this, "https://streamer.bot" + data.image_url);
                    }
                } else {
                    Debug.Log("Angry image data for Button {data.name}", gameObject);
                }
            } else {
                Debug.Log("Invalid Created time on Button {data.name}", gameObject);
            }
        }
    }
}
