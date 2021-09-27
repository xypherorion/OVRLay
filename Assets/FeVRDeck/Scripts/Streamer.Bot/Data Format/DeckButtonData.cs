using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Streamer.Bot.Data {
    /*
            {"id":11,
             "index":0,
             "name":"Test",
             "action_id":"b212c269-af8f-4390-badb-524225b175ef",
             "action_args":null,
             "background_color":"#DD17F1FF",
             "image_url":"/static/public/decks/1/item-11.png",
             "image_path":"/app/data/static/decks/1/item-11.png",
             "created_at":"2021-09-23T14:36:57.874Z"}
    */

    [JsonObject]
    public struct DeckButton {
        public uint id;
        public int index;
        public string name;
        public string action_id;
        public string[] action_args;
        public string background_color;
        public string image_url;
        public string image_path;
        public string created_at;
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }

    public static class DeckButtonDataSerializer {
        public static DeckButton Deserialize(string json) {
            return JsonConvert.DeserializeObject<DeckButton> (json);
        }

        public static string Serialize(this DeckButton deck) {
            return JsonConvert.SerializeObject(deck);
        }
    }
}