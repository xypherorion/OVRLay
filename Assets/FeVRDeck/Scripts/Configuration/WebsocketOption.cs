using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Streamer.Bot {

    public class WebsocketOption : MonoBehaviour {
        string WebsocketHost;
        int WebsocketPort;
        public InputField HostField;
        public InputField PortField;

        WebSocketClient client;

        public void Start() {
            WebsocketHost = PlayerPrefs.GetString("WebsocketHost", "localhost");
            WebsocketPort = PlayerPrefs.GetInt("WebsocketPort", 8080);

            if(client = FindObjectOfType<WebSocketClient>()) {
                client.host = WebsocketHost;
                client.port = WebsocketPort;
            }

            if (HostField)
                HostField.text = WebsocketHost;
            if (PortField)
                PortField.text = WebsocketPort.ToString();
        }

        public void SetHost(string host) {
            //Match Regex for Hostname and IP Address 
            if (Regex.IsMatch(host, @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$") ||
                Regex.IsMatch(host, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$")) {
                WebsocketHost = host;
                PlayerPrefs.SetString("WebsocketHost", WebsocketHost);
            } else {
                Debug.LogError($"\"{host}\" failed Regex for a Hostname");
            }
        }

        public void SetPort(string port) {
            int p = 8080;
            if (int.TryParse(port, out p)) {
                WebsocketPort = p;
                PlayerPrefs.SetInt("WebsocketPort", WebsocketPort);
            } else {
                Debug.LogError($"Unable to parse \"{port}\" as int");
            }
        }
    }
}
