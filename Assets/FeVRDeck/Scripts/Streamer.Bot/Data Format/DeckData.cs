using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Streamer.Bot.Data {

    /*
     {
        "id":1,
        "public_id":"49a1ef0e-461e-4ca7-9b68-44e2ee4956e6",
        "name":"GayDeck3000",
        "host":"192.168.1.50",
        "port":9001,
        "endpoint":"/",
        "num_rows":3,
        "num_cols":5,
        "styles":{},
        "items":[
            {"id":11,
             "index":0,
             "name":"Test",
             "action_id":"b212c269-af8f-4390-badb-524225b175ef",
             "action_args":null,
             "background_color":"#DD17F1FF",
             "image_url":"/static/public/decks/1/item-11.png",
             "image_path":"/app/data/static/decks/1/item-11.png",
             "created_at":"2021-09-23T14:36:57.874Z"}
        ],
        "created_at":"2021-09-22T21:36:57.582Z"}
     * */

    [JsonObject]
    public struct Deck {
        public uint id;
        public string public_id;
        public string name;
        public string host;
        public uint port;
        public string endpoint;
        public int num_rows;
        public int num_cols;
        public object styles;
        public DeckButton[] items;
        public string created_at; //System.DateTime

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }

    public static class DeckDataSerializer {
        public static Deck Deserialize(string json) {
            return JsonConvert.DeserializeObject<Deck>(json);
        }

        public static string Serialize(this Deck deck) {
            return JsonConvert.SerializeObject(deck);
        }

        public static string Serialize(this Streamer.Bot.Deck deck) {
            return JsonConvert.SerializeObject(deck.data);
        }
    }
}
