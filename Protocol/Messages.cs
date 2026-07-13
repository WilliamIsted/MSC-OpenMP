namespace MscOpenMp.Protocol
{
    public static class ProtocolInfo { public const byte Version = 1; }

    public static class MsgType
    {
        public const byte Hello = 0x01, Welcome = 0x02, Reject = 0x03,
            PeerJoined = 0x04, PeerLeft = 0x05, ChatSend = 0x06, Chat = 0x07,
            Relay = 0x08, Relayed = 0x09;
    }

    public static class Channels { public const byte ReliableEvent = 0x01, StateSnapshot = 0x02; }

    public struct PeerInfo { public uint Id; public string Name; public string ClientVersion; }

    public class Hello
    {
        public byte Version; public string ClientVersion; public string Name;
        public byte[] Encode()
        {
            var w = new PacketWriter(); w.U8(Version); w.Str(ClientVersion); w.Str(Name);
            return Frame.Encode(MsgType.Hello, w.ToArray());
        }
        public static Hello Decode(PacketReader r) =>
            new Hello { Version = r.U8(), ClientVersion = r.Str(), Name = r.Str() };
    }

    public class Welcome
    {
        public uint PeerId; public PeerInfo[] Peers;
        public byte[] Encode()
        {
            var w = new PacketWriter(); w.U32(PeerId); w.U8((byte)Peers.Length);
            foreach (var p in Peers) { w.U32(p.Id); w.Str(p.Name); w.Str(p.ClientVersion); }
            return Frame.Encode(MsgType.Welcome, w.ToArray());
        }
        public static Welcome Decode(PacketReader r)
        {
            var m = new Welcome { PeerId = r.U32(), Peers = new PeerInfo[r.U8()] };
            for (int i = 0; i < m.Peers.Length; i++)
                m.Peers[i] = new PeerInfo { Id = r.U32(), Name = r.Str(), ClientVersion = r.Str() };
            return m;
        }
    }

    public class Reject
    {
        public string Reason;
        public byte[] Encode()
        { var w = new PacketWriter(); w.Str(Reason); return Frame.Encode(MsgType.Reject, w.ToArray()); }
        public static Reject Decode(PacketReader r) => new Reject { Reason = r.Str() };
    }

    public class PeerJoined
    {
        public uint Id; public string Name; public string ClientVersion;
        public byte[] Encode()
        {
            var w = new PacketWriter(); w.U32(Id); w.Str(Name); w.Str(ClientVersion);
            return Frame.Encode(MsgType.PeerJoined, w.ToArray());
        }
        public static PeerJoined Decode(PacketReader r) =>
            new PeerJoined { Id = r.U32(), Name = r.Str(), ClientVersion = r.Str() };
    }

    public class PeerLeft
    {
        public uint Id;
        public byte[] Encode()
        { var w = new PacketWriter(); w.U32(Id); return Frame.Encode(MsgType.PeerLeft, w.ToArray()); }
        public static PeerLeft Decode(PacketReader r) => new PeerLeft { Id = r.U32() };
    }

    public class ChatSend
    {
        public string Text;
        public byte[] Encode()
        { var w = new PacketWriter(); w.Str(Text); return Frame.Encode(MsgType.ChatSend, w.ToArray()); }
        public static ChatSend Decode(PacketReader r) => new ChatSend { Text = r.Str() };
    }

    public class Chat
    {
        public uint FromId; public string Text;
        public byte[] Encode()
        { var w = new PacketWriter(); w.U32(FromId); w.Str(Text); return Frame.Encode(MsgType.Chat, w.ToArray()); }
        public static Chat Decode(PacketReader r) => new Chat { FromId = r.U32(), Text = r.Str() };
    }

    public class Relay
    {
        public byte Channel; public byte[] Payload;
        public byte[] Encode()
        { var w = new PacketWriter(); w.U8(Channel); w.Raw(Payload); return Frame.Encode(MsgType.Relay, w.ToArray()); }
        public static Relay Decode(PacketReader r) => new Relay { Channel = r.U8(), Payload = r.Rest() };
    }

    public class Relayed
    {
        public uint FromId; public byte Channel; public byte[] Payload;
        public byte[] Encode()
        {
            var w = new PacketWriter(); w.U32(FromId); w.U8(Channel); w.Raw(Payload);
            return Frame.Encode(MsgType.Relayed, w.ToArray());
        }
        public static Relayed Decode(PacketReader r) =>
            new Relayed { FromId = r.U32(), Channel = r.U8(), Payload = r.Rest() };
    }
}
