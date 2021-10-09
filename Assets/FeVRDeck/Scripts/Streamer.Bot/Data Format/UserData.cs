using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Streamer.Bot.Data {

    public struct UserData {
        public string displayName;
        public string name;
        private string authKey;
        public Deck[] decks;
    }
}
