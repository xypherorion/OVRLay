using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Streamer.Bot {

    public class DeckOption : MonoBehaviour {
        public Toggle OptionToggle;
        public InputField PublicDeckKeyField;

        public string SettingsKey = "";
        public string PublicDeckKey = "";

        public Deck deck;
        public Unity_Overlay deckOverlayTarget;

        public async void Start() {
            if (!string.IsNullOrEmpty(SettingsKey) && OptionToggle && PublicDeckKeyField && deck) {
                PublicDeckKey = PublicDeckKeyField.text = PlayerPrefs.GetString(SettingsKey, "");
                bool toggleOn = PlayerPrefs.GetString(SettingsKey + "Enabled", false.ToString()) == true.ToString();
                OptionToggle.onValueChanged.Invoke(toggleOn);

                if(!string.IsNullOrEmpty(PublicDeckKey))
                    await deck.Load(PublicDeckKey);
                
            } else {
                Debug.LogError("You forgot to assign something", gameObject);
                if (deckOverlayTarget)
                    deckOverlayTarget.isVisible = false;
            }
        }

        public void UpdateOption(bool toggle) {
            if(OptionToggle)
                PlayerPrefs.SetString(SettingsKey + "Enabled", toggle.ToString());

            if (deckOverlayTarget)
                deckOverlayTarget.isVisible = toggle;
        }

        public async void UpdateKey(string key) {
            //Sanitize in case some derp put a whole URL in here
            //This should probably be a RegEx
            key = key.Split('/').Last();

            PublicDeckKeyField.text = PublicDeckKey = key;
            PlayerPrefs.SetString(SettingsKey, PublicDeckKey);
            if (!string.IsNullOrEmpty(PublicDeckKey))
                await deck.Load(PublicDeckKey);

        }
    }
}
