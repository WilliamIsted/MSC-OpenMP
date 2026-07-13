using System;
using System.IO;
using System.Text;

namespace MscOpenMp.Protocol
{
    public class PacketReader
    {
        readonly byte[] _d;
        int _p;

        public PacketReader(byte[] data) { _d = data; }
        public bool End { get { return _p >= _d.Length; } }

        void Need(int n) { if (_p + n > _d.Length) throw new EndOfStreamException(); }

        public byte U8() { Need(1); return _d[_p++]; }
        public ushort U16() { Need(2); var v = (ushort)(_d[_p] | (_d[_p + 1] << 8)); _p += 2; return v; }
        public uint U32() { return (uint)(U16() | (U16() << 16)); }
        public float F32() { Need(4); var v = System.BitConverter.ToSingle(_d, _p); _p += 4; return v; }
        public string Str()
        {
            int len = U16(); Need(len);
            var s = Encoding.UTF8.GetString(_d, _p, len); _p += len; return s;
        }
        public byte[] Rest()
        {
            var r = new byte[_d.Length - _p];
            Buffer.BlockCopy(_d, _p, r, 0, r.Length); _p = _d.Length; return r;
        }
    }
}
