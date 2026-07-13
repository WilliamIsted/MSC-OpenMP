using System.Collections.Generic;
using MscOpenMp.Mod.Net;
using MscOpenMp.Protocol;
using UnityEngine;

namespace MscOpenMp.Mod.Ui
{
    public class ConnectChatUi
    {
        readonly InboundQueue _queue = new InboundQueue();
        ITransport _transport;
        string _url = "ws://127.0.0.1:8380/game";
        string _name = "Player";
        string _chatInput = "";
        readonly List<string> _log = new List<string>();
        readonly Dictionary<uint, string> _names = new Dictionary<uint, string>();
        bool _connected;
        Rect _win = new Rect(20, 20, 420, 320);
        Vector2 _scroll;

        public void Pump() { _queue.DrainOnMainThread(); }

        void Log(string s) { _log.Add(s); if (_log.Count > 200) _log.RemoveAt(0); }

        void ConnectClicked()
        {
            var t = new WsTransport();
            _transport = t;
            t.Connected += () => _queue.Post(() =>
                t.Send(new Hello { Version = ProtocolInfo.Version, ClientVersion = ModInfo.ClientVersion, Name = _name }.Encode()));
            t.FrameReceived += bytes => _queue.Post(() => OnFrame(bytes));
            t.Closed += reason => _queue.Post(() => { _connected = false; Log("Disconnected: " + reason); });
            t.Connect(_url);
            Log("Connecting to " + _url + " ...");
        }

        void OnFrame(byte[] frame)
        {
            byte type; byte[] payload; int off = 0;
            if (!Frame.TryDecode(frame, frame.Length, ref off, out type, out payload)) return;
            var r = new PacketReader(payload);
            switch (type)
            {
                case MsgType.Welcome:
                    var w = Welcome.Decode(r);
                    foreach (var p in w.Peers)
                        if (p.ClientVersion != ModInfo.ClientVersion)
                        {
                            // client-compat rule v1: exact match; relay doesn't enforce, we do
                            Log("Version mismatch: " + p.Name + " runs " + p.ClientVersion + ", you run " + ModInfo.ClientVersion);
                            _transport.Close();
                            return;
                        }
                    _connected = true;
                    _names.Clear();
                    foreach (var p in w.Peers) _names[p.Id] = p.Name;
                    Log("Connected. Players online: " + (w.Peers.Length + 1));
                    break;
                case MsgType.Reject:
                    Log("Rejected: " + Reject.Decode(r).Reason);
                    break;
                case MsgType.PeerJoined:
                    var pj = PeerJoined.Decode(r);
                    _names[pj.Id] = pj.Name;
                    Log(pj.Name + " joined" + (pj.ClientVersion != ModInfo.ClientVersion ? " (INCOMPATIBLE " + pj.ClientVersion + ")" : ""));
                    break;
                case MsgType.PeerLeft:
                    var pl = PeerLeft.Decode(r);
                    string name;
                    if (_names.TryGetValue(pl.Id, out name)) { Log(name + " left"); _names.Remove(pl.Id); }
                    break;
                case MsgType.Chat:
                    var c = Chat.Decode(r);
                    string from;
                    if (!_names.TryGetValue(c.FromId, out from)) from = "You";
                    Log(from + ": " + c.Text);
                    break;
                // MsgType.Relayed intentionally ignored in M1 — sync plans consume it
            }
        }

        void SendChat()
        {
            if (_transport == null || _chatInput.Length == 0) return;
            _transport.Send(new ChatSend { Text = _chatInput }.Encode());
            _chatInput = "";
        }

        public void Draw()
        {
            _win = GUI.Window(0x0313, _win, DrawWindow, "MSC OpenMP");
        }

        void DrawWindow(int id)
        {
            if (!_connected)
            {
                GUILayout.Label("Server:"); _url = GUILayout.TextField(_url);
                GUILayout.Label("Name:"); _name = GUILayout.TextField(_name);
                if (GUILayout.Button("Connect")) ConnectClicked();
            }
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(180));
            foreach (var line in _log) GUILayout.Label(line);
            GUILayout.EndScrollView();
            if (_connected)
            {
                GUILayout.BeginHorizontal();
                _chatInput = GUILayout.TextField(_chatInput);
                if (GUILayout.Button("Send", GUILayout.Width(60))) SendChat();
                GUILayout.EndHorizontal();
            }
            GUI.DragWindow();
        }
    }
}
