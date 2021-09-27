using Streamer.Bot.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

namespace Streamer.Bot {

    public class Deck : MonoBehaviour {
        //Example:
        //https://streamer.bot/api/decks-public/49a1ef0e-461e-4ca7-9b68-44e2ee4956e6 
        public Data.Deck data;
        private List<DeckButton> buttons = new List<DeckButton>();

        public string PublicDeckId = "";
        public DeckButton DeckButtonPrefab;
        public Transform ButtonParentTransform;

        //UI Elements
        public CanvasGroup canvasGroup;
        public Unity_Overlay overlay;

        public Deck() : base() {}
        public Deck(Data.Deck data) : base() {
            this.data = data;
        }

        async public void Start() {
            if (!string.IsNullOrWhiteSpace(PublicDeckId)) {
                data = await DownloadPublicDeck("https://streamer.bot/api/decks-public/" + PublicDeckId);
                Build();
            } else Debug.LogWarning("Public ID is NULL", gameObject);
        }


        public async Task<Data.Deck> DownloadPublicDeck(string url) {
            Debug.Log($"Downloading Public Deck {url}");

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
                    Debug.Log($"{www.error}, URL:{www.url}");
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
            //Check if data is valid by checkind creation timestamp
            if (data.created_at != default) {
                name = data.name;
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
                Debug.LogError("DeckData is invalid", gameObject);
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
