namespace MscOpenMp.Protocol
{
    // RELAY channel 0x02 payload body. Game-plane: relay never parses this.
    // Layout: [u16 seq][f32 x][f32 y][f32 z][f32 yaw][f32 pitch][u8 stance][u32 sentAtMs] = 27 bytes.
    public class PlayerSnapshot
    {
        public ushort Seq;
        public float PosX, PosY, PosZ;
        public float Yaw, Pitch;
        public byte Stance;   // camera height above root, metres x 100 (stand ~140, crouch ~85, prone ~30)
        public uint SentAtMs; // wall clock, Unix ms truncated to u32; deltas wrap-safe via uint subtraction

        public byte[] EncodePayload()
        {
            var w = new PacketWriter();
            w.U16(Seq); w.F32(PosX); w.F32(PosY); w.F32(PosZ);
            w.F32(Yaw); w.F32(Pitch); w.U8(Stance); w.U32(SentAtMs);
            return w.ToArray();
        }

        public static PlayerSnapshot DecodePayload(PacketReader r)
        {
            return new PlayerSnapshot
            {
                Seq = r.U16(),
                PosX = r.F32(), PosY = r.F32(), PosZ = r.F32(),
                Yaw = r.F32(), Pitch = r.F32(),
                Stance = r.U8(),
                SentAtMs = r.U32()
            };
        }
    }
}
