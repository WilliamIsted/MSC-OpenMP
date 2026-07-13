using System.IO;
using MscOpenMp.Protocol;
using Xunit;

public class PacketIoTests
{
    [Fact]
    public void RoundTrips_AllPrimitives()
    {
        var w = new PacketWriter();
        w.U8(7); w.U16(65535); w.U32(0xDEADBEEF); w.F32(13.5f); w.Str("Satsuma ÄÖ"); w.Raw(new byte[] { 1, 2 });
        var r = new PacketReader(w.ToArray());
        Assert.Equal(7, r.U8());
        Assert.Equal(65535, r.U16());
        Assert.Equal(0xDEADBEEF, r.U32());
        Assert.Equal(13.5f, r.F32());
        Assert.Equal("Satsuma ÄÖ", r.Str());
        Assert.Equal(new byte[] { 1, 2 }, r.Rest());
        Assert.True(r.End);
    }

    [Fact]
    public void U32_IsLittleEndian()
    {
        var w = new PacketWriter();
        w.U32(0x04030201);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, w.ToArray());
    }

    [Fact]
    public void OverRead_Throws()
    {
        var r = new PacketReader(new byte[] { 1 });
        r.U8();
        Assert.Throws<EndOfStreamException>(() => r.U16());
    }
}
