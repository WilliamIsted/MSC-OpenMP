using System.IO;
using System.Text;

namespace MscOpenMp.Protocol
{
    public class PacketWriter
    {
        readonly MemoryStream _ms = new MemoryStream();

        public void U8(byte v) { _ms.WriteByte(v); }
        public void U16(ushort v) { _ms.WriteByte((byte)v); _ms.WriteByte((byte)(v >> 8)); }
        public void U32(uint v) { U16((ushort)v); U16((ushort)(v >> 16)); }
        public void F32(float v)
        {
            // ponytail: BitConverter is LE on every platform MSC runs on; asserted by fixture tests
            var b = System.BitConverter.GetBytes(v);
            _ms.Write(b, 0, 4);
        }
        public void Str(string v)
        {
            var b = Encoding.UTF8.GetBytes(v ?? "");
            U16((ushort)b.Length);
            _ms.Write(b, 0, b.Length);
        }
        public void Raw(byte[] v) { _ms.Write(v, 0, v.Length); }
        public byte[] ToArray() { return _ms.ToArray(); }
    }
}
