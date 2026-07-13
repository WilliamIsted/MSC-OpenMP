using MscOpenMp.Protocol;
using Xunit;

public class PlayerSnapshotTests
{
    [Fact]
    public void RoundTrips()
    {
        var s = new PlayerSnapshot { Seq = 41, PosX = 1.5f, PosY = -2f, PosZ = 300.25f, Yaw = 181.5f, Pitch = -10f, Stance = 140 };
        var d = PlayerSnapshot.DecodePayload(new PacketReader(s.EncodePayload()));
        Assert.Equal((ushort)41, d.Seq);
        Assert.Equal(300.25f, d.PosZ);
        Assert.Equal(181.5f, d.Yaw);
        Assert.Equal(140, d.Stance);
    }

    [Fact]
    public void Payload_Is23Bytes()
    {
        Assert.Equal(23, new PlayerSnapshot().EncodePayload().Length);
    }

    [Fact]
    public void Seq_Fixture()
    {
        var s = new PlayerSnapshot { Seq = 0x0201 };
        Assert.Equal(0x01, s.EncodePayload()[0]);
        Assert.Equal(0x02, s.EncodePayload()[1]);
    }
}
