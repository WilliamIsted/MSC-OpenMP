using System;
using MscOpenMp.Protocol;
using Xunit;

public class FrameTests
{
    [Fact]
    public void EncodeDecode_RoundTrips()
    {
        var f = Frame.Encode(6, new byte[] { 9, 9 });
        Assert.Equal(new byte[] { 6, 2, 0, 9, 9 }, f);
        int off = 0; byte type; byte[] payload;
        Assert.True(Frame.TryDecode(f, f.Length, ref off, out type, out payload));
        Assert.Equal(6, type);
        Assert.Equal(new byte[] { 9, 9 }, payload);
        Assert.Equal(5, off);
    }

    [Fact]
    public void TryDecode_Incomplete_ReturnsFalse()
    {
        var f = Frame.Encode(6, new byte[] { 9, 9 });
        int off = 0; byte t; byte[] p;
        Assert.False(Frame.TryDecode(f, 4, ref off, out t, out p));
        Assert.Equal(0, off);
    }

    [Fact]
    public void Encode_Oversize_Throws() =>
        Assert.Throws<ArgumentException>(() => Frame.Encode(1, new byte[65536]));
}
