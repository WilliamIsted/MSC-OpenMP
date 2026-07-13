using System;

namespace MscOpenMp.Mod.Net
{
    public interface ITransport
    {
        event Action Connected;
        event Action<byte[]> FrameReceived;
        event Action<string> Closed;
        void Connect(string url);
        void Send(byte[] frame);
        void Close();
    }
}
