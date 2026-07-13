using System;

namespace MscOpenMp.Protocol
{
    public static class Frame
    {
        public static byte[] Encode(byte type, byte[] payload)
        {
            if (payload.Length > ushort.MaxValue) throw new ArgumentException("payload > 64KB; chunk it");
            var f = new byte[3 + payload.Length];
            f[0] = type; f[1] = (byte)payload.Length; f[2] = (byte)(payload.Length >> 8);
            Buffer.BlockCopy(payload, 0, f, 3, payload.Length);
            return f;
        }

        public static bool TryDecode(byte[] buffer, int count, ref int offset, out byte type, out byte[] payload)
        {
            type = 0; payload = null;
            if (count - offset < 3) return false;
            int len = buffer[offset + 1] | (buffer[offset + 2] << 8);
            if (count - offset < 3 + len) return false;
            type = buffer[offset];
            payload = new byte[len];
            Buffer.BlockCopy(buffer, offset + 3, payload, 0, len);
            offset += 3 + len;
            return true;
        }
    }
}
