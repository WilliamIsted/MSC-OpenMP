using System;
using MscOpenMp.Mod.Net;
using MscOpenMp.Protocol;
using UnityEngine;

namespace MscOpenMp.Mod.Sync
{
    // Samples the local PLAYER rig and streams 10 Hz snapshots over RELAY channel 2.
    public class PlayerSync
    {
        const float SendInterval = 0.1f; // 10 Hz
        // Rig paths confirmed by Task 1 live probe (2026-07-14):
        const string PlayerPath = "PLAYER";                                    // position + yaw authority
        const string CameraPath = "PLAYER/Pivot/AnimPivot/Camera/FPSCamera";   // pitch + stance height

        readonly Func<ITransport> _transport;
        readonly Func<bool> _connected;
        Transform _player, _camera;
        float _nextSend;
        ushort _seq;

        public PlayerSync(Func<ITransport> transport, Func<bool> connected)
        { _transport = transport; _connected = connected; }

        // lazy + world-reload-safe: re-find when references die
        bool Resolve()
        {
            if (_player == null)
            {
                var go = GameObject.Find(PlayerPath);
                if (go == null) return false;
                _player = go.transform;
                _camera = null;
            }
            if (_camera == null)
            {
                var cam = GameObject.Find(CameraPath);
                if (cam != null) _camera = cam.transform;
            }
            return _camera != null;
        }

        public void Tick()
        {
            if (!_connected() || Time.time < _nextSend || !Resolve()) return;
            _nextSend = Time.time + SendInterval;
            var snap = new PlayerSnapshot
            {
                Seq = ++_seq,
                PosX = _player.position.x, PosY = _player.position.y, PosZ = _player.position.z,
                Yaw = _player.eulerAngles.y,
                Pitch = _camera.localEulerAngles.x,
                Stance = (byte)Mathf.Clamp(Mathf.RoundToInt((_camera.position.y - _player.position.y) * 100f), 0, 255),
                SentAtMs = Clock.NowMs()
            };
            var t = _transport();
            if (t != null)
                t.Send(new Relay { Channel = Channels.StateSnapshot, Payload = snap.EncodePayload() }.Encode());
        }
    }
}
