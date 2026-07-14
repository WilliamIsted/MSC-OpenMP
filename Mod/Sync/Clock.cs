using System;

namespace MscOpenMp.Mod.Sync
{
    // Shared wall clock for snapshot latency stamps. Unix ms truncated to u32;
    // compare only via wrap-safe uint subtraction. Exact when both clients share
    // a machine/NTP; skew-limited accuracy across machines.
    public static class Clock
    {
        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static uint NowMs() { return (uint)(long)(DateTime.UtcNow - Epoch).TotalMilliseconds; }
    }
}
