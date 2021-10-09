using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Streamer.Bot {


    public class WebAuth : MonoBehaviour {
        const string webReqURL = "https://streamer.bot/api/auth/discord";

        async void Start() {
            Application.OpenURL(webReqURL);
            await RequestWebAuth();
        }

        public async Task<string> RequestWebAuth() {
        
            Debug.Log($"Requesting Authorization {webReqURL}");

            using (UnityWebRequest www = UnityWebRequest.Get(webReqURL)) {
                // begin request:
                var asyncOp = www.SendWebRequest();

                // await until it's done: 
                while (asyncOp.isDone == false)
                    await Task.Delay(500);

                string response = default;
                // read results:
                if (www.result != UnityWebRequest.Result.Success) {
                    // log error:
                    Debug.Log($"{www.error}, URL:{www.url}");
                } else {
                    // return valid results:
                    try {
                        response = DownloadHandlerBuffer.GetContent(www);
                    } catch (Exception e) {
                        Debug.LogError(e.Message, gameObject);
                    }
                }

                Debug.Log("Authorization Response: " + response);
                return response;
            }
        }
        void Update() {

        }
    }

    public struct WebAuthData {
        public string name;
        public string apiKey;
    }
}