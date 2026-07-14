using System.Collections.Generic;
using MscOpenMp.Protocol;

namespace MscOpenMp.Mod.Sync
{
    // Peer lifecycle -> ghost lifecycle.
    public class RemotePlayerRegistry
    {
        readonly Dictionary<uint, RemotePlayer> _players = new Dictionary<uint, RemotePlayer>();

        public void PeerJoined(uint id, string name)
        { PeerLeft(id); _players[id] = new RemotePlayer(id, name); }

        public void PeerLeft(uint id)
        {
            RemotePlayer p;
            if (_players.TryGetValue(id, out p)) { p.Destroy(); _players.Remove(id); }
        }

        public void Clear()
        {
            foreach (var kv in _players) kv.Value.Destroy();
            _players.Clear();
        }

        public void OnSnapshot(uint fromId, PlayerSnapshot s)
        {
            RemotePlayer p;
            if (_players.TryGetValue(fromId, out p)) p.OnSnapshot(s);
        }

        public void Tick() { foreach (var kv in _players) kv.Value.Tick(); }
    }
}
