using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Concurrent;
using System.Text;

namespace Streamer.Bot {

    public class WebSocketClient : MonoBehaviour {
        public string host = "localhost";
        public int port = 8080;
        public string endpoint = "/";
        ClientWebSocket webCli;

        public GameObject DisconnectedObj;
        public GameObject ConnectedObj;

        public string[] GeneralSubscriptions = new string[] { 
            "Custom"
        };
        public string[] TwitchEventSubscriptions = new string[] {
            "Follow",
            "Cheer",
            "Sub",
            "ReSub",
            "GiftSub",
            "GiftBomb",
            "Raid",
            "HypeTrainStart",
            "HypeTrainUpdate",
            "HypeTrainLevelUp",
            "HypeTrainEnd",
            "RewardRedemption",
            "RewardCreated",
            "RewardUpdated",
            "RewardDeleted",
            "CommunityGoalContribution",
            "CommunityGoalEnded",
            "StreamUpdate",
            "Whisper",
            "FirstWord",
            "SubCounterRollover",
            "BroadcastUpdate",
            "StreamUpdateGameOnConnect",
            "PresentViewers",
            "PollCreated",
            "PollUpdated",
            "PollCompleted",
            "PredictionCreated",
            "PredictionUpdated",
            "PredictionCompleted",
            "PredictionCanceled",
            "PredictionLocked",
            "ChatMessage"
        };

        public string[] StreamlabsSubscriptions = new string[] { 
            "Donation",
            "Merchandise"
        };

        public string[] SpeechToTextSubscriptions = new string[] {
            "Dictation",
            "Command"
        };

        public string[] CommandSubscriptions = new string[] {
            "Message",
            "Whisper"
        };

        public string[] FileWatcherSubscriptions = new string[] {
            "Changed",
            "Created",
            "Deleted"
        };

        public string[] QuoteSubscriptions = new string[] {
            "Added",
            "Show"
        };

        public string[] MiscSubscriptions = new string[] {
            "TimedAction",
            "PyramidSuccess"
        };

        public string[] RawSubscriptions = new string[] {
            "Action",
            "SubAction"
        };

        public string[] WebSocketClientSubscriptions = new string[] {
            "Open",
            "Close",
            "Message"
        };

        public string[] StreamElementsSubscriptions = new string[] {
            "Tip"
        };

        private static object consoleLock = new object();
        private const int sendChunkSize = 256;
        private const int receiveChunkSize = 64;
        private const bool verbose = true;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(1000);


        async void Start() {
            if(webCli == null)
                await Connect($"ws://{host}:{port}{endpoint}");

            Subscribe();
        }

        public async Task Connect(string uri) {
            webCli = null;

            try {
                Debug.Log($"Connecting to Websocket {uri}");
                webCli = new ClientWebSocket();
                await webCli.ConnectAsync(new Uri(uri), CancellationToken.None);

                if (DisconnectedObj)
                    DisconnectedObj.SetActive(false);
                if (ConnectedObj)
                    ConnectedObj.SetActive(true);

                await Task.WhenAll(this.OnReceive(webCli), this.OnSend(webCli));
            } catch (Exception ex) {
                Debug.Log($"Exception: {ex.Message}");
            } finally {
                if (webCli != null)
                    webCli.Dispose();
                Console.WriteLine();

                lock (consoleLock) {
                    Debug.Log("<color='red'>WebSocket closed.</color>");
                    if (DisconnectedObj)
                        DisconnectedObj.SetActive(true);
                    if (ConnectedObj)
                        ConnectedObj.SetActive(false);
                }
            }
        }


        private byte[] buffer = new byte[sendChunkSize];
        public void LoadBuffer(string strBuf) {
            LoadBuffer(strBuf.GetUTF8EncodedBytes());
        }
        public void LoadBuffer(byte[] buf) {
            buffer = buf;
        }

        private void Subscribe() {
            string SubEventStr =
            "{" +
                "\"id\": 0," +
                "\"request\": \"Subscribe\"," +
                "\"events\":{"+
                    $"\"general\":[{GeneralSubscriptions}]" +
                    $"\"twitch\":[{TwitchEventSubscriptions}]" +
                    $"\"streamlabs\":[{StreamlabsSubscriptions}]" +
                    $"\"speechToText\":[{SpeechToTextSubscriptions}]" +
                    $"\"command\":[{CommandSubscriptions}]" +
                    $"\"fileWatcher\":[{FileWatcherSubscriptions}]" +
                    $"\"quote\":[{QuoteSubscriptions}]" +
                    $"\"misc\":[{MiscSubscriptions}]" +
                    $"\"raw\":[{RawSubscriptions}]" +
                    $"\"websocketClient\":[{WebSocketClientSubscriptions}]" +
                    $"\"streamElements\":[{StreamElementsSubscriptions}]" +
                "}" +
            "}";
            CommandQueue.Enqueue(SubEventStr);
        }


        public struct DoCommandData {
            public string id;
            public string name;
            public string args;
        }

        private int msgId = 0;
        protected int NextMessage {
            get {
                int id = msgId;
                msgId++;
                return id;
            }
        }

        public string Cmd_DoAction(DoCommandData cmd) {
            string r =
            "{" +
                "\"request\": \"DoAction\"," +
                "\"action\": {" +
                    $"\"id\": \"{cmd.id}\"," +
                    $"\"name\": \"{cmd.name}\"" +
                "}," +
                "\"args\": {" +
                    cmd.args + //"\"key\": \"value\"," +
                "}," +
                $"\"id\": \"{NextMessage}\"" +
            "}";
            Debug.Log("Command JSON:\n" + r);

            return r;
        }

        ConcurrentQueue<string> CommandQueue = new ConcurrentQueue<string>();
        string cmd;
        private async Task OnSend(ClientWebSocket webSocket) {
            while (webSocket.State == System.Net.WebSockets.WebSocketState.Open) {
                if (buffer != null && buffer.Length > 0) {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
                    buffer = null;
                    LogStatus(false, buffer, buffer.Length);
                }

                if(CommandQueue.Count > 0) {
                    while(CommandQueue.TryDequeue(out cmd)) {
                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(cmd)), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }

                await Task.Delay(delay);
            }
        }

        private async Task OnReceive(ClientWebSocket webSocket) {
            byte[] buffer = new byte[receiveChunkSize];

            while (webSocket.State == System.Net.WebSockets.WebSocketState.Open) {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                } else {
                    string recv = buffer.GetUTF8DecodedString();
                    LogStatus(true, buffer, result.Count);
                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length) {
            lock (consoleLock) {
                Debug.Log($"<color='{(receiving ? Color.green : Color.gray )}'>{0} {1} bytes... {(receiving ? "Received" : "Sent")} {length}</color>");

                if (verbose)
                    Debug.Log(BitConverter.ToString(buffer, 0, length));
            }
        }

        public void SendCommand(string command_id, string command_name, string command_args) {
            Debug.Log($"Sending [{command_id}] {command_name} {command_args}");
            CommandQueue.Enqueue(Cmd_DoAction(new DoCommandData() { id = command_id, name = command_name, args = command_args }));
        }
    }
}