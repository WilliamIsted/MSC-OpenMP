using MscOpenMp.Protocol;
using Xunit;

public class MessageFixtureTests
{
    static string Hex(byte[] b) => System.Convert.ToHexString(b);

    [Fact]
    public void Hello_Fixture()
    {
        var f = new Hello { Version = 1, ClientVersion = "0.1.0", Name = "Toivo" }.Encode();
        Assert.Equal("01" + "0F00" + "01" + "0500" + "302E312E30" + "0500" + "546F69766F", Hex(f));
    }

    [Fact]
    public void Chat_Fixture()
    {
        var f = new Chat { FromId = 2, Text = "moro" }.Encode();
        Assert.Equal("07" + "0A00" + "02000000" + "0400" + "6D6F726F", Hex(f));
    }

    [Fact]
    public void Relayed_Fixture()
    {
        var f = new Relayed { FromId = 1, Channel = Channels.StateSnapshot, Payload = new byte[] { 0xAA, 0xBB } }.Encode();
        Assert.Equal("09" + "0700" + "01000000" + "02" + "AABB", Hex(f));
    }

    [Fact]
    public void Welcome_RoundTrips()
    {
        var w = new Welcome { PeerId = 3, Peers = new[] {
            new PeerInfo { Id = 1, Name = "a", ClientVersion = "0.1.0" },
            new PeerInfo { Id = 2, Name = "b", ClientVersion = "0.2.0" } } };
        var frame = w.Encode();
        int off = 0; byte type; byte[] payload;
        Assert.True(Frame.TryDecode(frame, frame.Length, ref off, out type, out payload));
        Assert.Equal(MsgType.Welcome, type);
        var d = Welcome.Decode(new PacketReader(payload));
        Assert.Equal(3u, d.PeerId);
        Assert.Equal("0.2.0", d.Peers[1].ClientVersion);
    }

    [Fact]
    public void Relay_RoundTrips_PayloadOpaque()
    {
        var frame = new Relay { Channel = Channels.ReliableEvent, Payload = new byte[] { 1, 2, 3 } }.Encode();
        int off = 0; byte t; byte[] p;
        Frame.TryDecode(frame, frame.Length, ref off, out t, out p);
        var d = Relay.Decode(new PacketReader(p));
        Assert.Equal(Channels.ReliableEvent, d.Channel);
        Assert.Equal(new byte[] { 1, 2, 3 }, d.Payload);
    }

    [Theory]
    [InlineData("reason text")]
    public void Reject_RoundTrips(string reason)
    {
        var frame = new Reject { Reason = reason }.Encode();
        int off = 0; byte t; byte[] p;
        Frame.TryDecode(frame, frame.Length, ref off, out t, out p);
        Assert.Equal(reason, Reject.Decode(new PacketReader(p)).Reason);
    }
}
