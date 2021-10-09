using Streamer.Bot.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;

namespace Streamer.Bot {

    public class Deck : MonoBehaviour {
        public static string PublicDecksURL = "https://streamer.bot/api/decks-public/";

        //Example:
        //https://streamer.bot/api/decks-public/49a1ef0e-461e-4ca7-9b68-44e2ee4956e6 
        public Data.Deck data;
        private List<DeckButton> buttons = new List<DeckButton>();

        public string DebugDeckId = "";
        public DeckButton DeckButtonPrefab;
        public Transform ButtonParentTransform;

        //UI Elements
        public CanvasGroup canvasGroup;

        public Deck() : base() {}
        public Deck(Data.Deck data) : base() {
            this.data = data;
        }

        async public void Start() {
            if (data.Equals(default)) {
                if (!string.IsNullOrWhiteSpace(DebugDeckId)) {
                    data = await DownloadPublicDeck(DebugDeckId);
                    Build();
                } else
                    Debug.LogWarning("No Debug Deck to Load", gameObject);
            }
        }

        async public Task Load(string publicId) {
            data = default;
            if (!string.IsNullOrWhiteSpace(publicId)) {
                data = await DownloadPublicDeck(publicId);
                Build();
            } else {
                Debug.LogWarning("Unloaded Deck", gameObject);
            }
        }

        public async Task<Data.Deck> DownloadPublicDeck(string publicDeckId) {
            if (string.IsNullOrWhiteSpace(publicDeckId)) {
                Debug.LogError("Requested to download null public deck");
                return default;
            }
            
            string url = PublicDecksURL + publicDeckId;
            Debug.Log("Downloading Public Deck " + url);

            using (UnityWebRequest www = UnityWebRequest.Get(url)) {
                // begin request:
                var asyncOp = www.SendWebRequest();

                // await until it's done: 
                while (asyncOp.isDone == false)
                    await Task.Delay(500);

                Data.Deck deckData = default;
                // read results:
                if (www.result != UnityWebRequest.Result.Success) {
                    // log error:
                    Debug.Log(www.error+ ", URL:" + url);
                } else {
                    // return valid results:
                    try {
                        string d = DownloadHandlerBuffer.GetContent(www);
                        Debug.Log(d);
                        deckData = Data.DeckDataSerializer.Deserialize(d);
                    } catch(Exception e) {
                        Debug.LogError(e.Message, gameObject);
                    }
                }

                return deckData;
            }
        }

        private void Build() {
            if (DeckButtonPrefab == null)
                Debug.LogError("Unable to build deck, Button Prefab is invalid");

            //Check if data is valid by checking creation timestamp
            if (data.created_at != default) {
                name = data.name;

                foreach (DeckButton btn in buttons)
                    Destroy(btn.gameObject);
                buttons.Clear();

                if (data.items != null && data.items.Length > 0) {
                    Data.DeckButton btnData;
                    DeckButton button;
                    for (int b = 0; b < data.items.Length; b++) {
                        btnData = data.items[b];
                        if (button = Instantiate(DeckButtonPrefab, ButtonParentTransform)) {
                            button.SetData(btnData);
                            buttons.Add(button);
                        }
                    }
                } else {
                    Debug.LogWarning("No Decks", gameObject);
                }
            } else {
                Debug.LogWarning(data.ToString());
                Debug.LogError("Deck created_at is invalid", gameObject);
            }
        }

        public void TestQueryDeck() {
        }
    }

    public static class DeckSerializer {
        public static Deck Deserialize(string json) {
            return new Deck(Data.DeckDataSerializer.Deserialize(json));
        }

        public static string Serialize(this Deck deck) {
            return JsonConvert.SerializeObject(deck.data);
        }
    }
}
