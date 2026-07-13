using System;
using WebSocketSharp;

namespace MscOpenMp.Mod.Net
{
    public class WsTransport : ITransport
    {
        public event Action Connected;
        public event Action<byte[]> FrameReceived;
        public event Action<string> Closed;
        WebSocket _ws;

        public void Connect(string url)
        {
            _ws = new WebSocket(url);
            _ws.OnOpen += (s, e) => { var h = Connected; if (h != null) h(); };
            _ws.OnMessage += (s, e) => { if (e.IsBinary) { var h = FrameReceived; if (h != null) h(e.RawData); } };
            _ws.OnClose += (s, e) => { var h = Closed; if (h != null) h(e.Reason ?? "closed"); };
            _ws.OnError += (s, e) => { var h = Closed; if (h != null) h(e.Message); };
            _ws.ConnectAsync(); // websocket-sharp's own thread; NOT TPL
        }

        public void Send(byte[] frame) { if (_ws != null && _ws.ReadyState == WebSocketState.Open) _ws.Send(frame); }
        public void Close() { if (_ws != null) _ws.CloseAsync(); }
    }
}
