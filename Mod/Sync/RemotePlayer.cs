using System.Collections.Generic;
using MscOpenMp.Protocol;
using UnityEngine;

namespace MscOpenMp.Mod.Sync
{
    // Per-peer ghost: snapshot buffer, 150 ms interpolation, capsule + billboarded nametag.
    public class RemotePlayer
    {
        struct Entry { public float T; public PlayerSnapshot S; }

        const float InterpDelay = 0.15f, ExtrapCap = 0.25f;
        readonly List<Entry> _buf = new List<Entry>(); // ordered by arrival; small (<=32)
        GameObject _rig; TextMesh _tag;
        ushort _lastSeq; bool _any;
        readonly string _name;
        float _shownDelayMs = -1;

        // ms between the sender stamping the rendered snapshot and now (network + interp buffer).
        // Exact on shared clocks (same machine); skew-limited across machines.
        public float ReplayDelayMs { get; private set; }

        public RemotePlayer(uint peerId, string name)
        {
            _name = name;
            _rig = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Object.Destroy(_rig.GetComponent<Collider>()); // ghost: never touch physics
            _rig.name = "MpGhost_" + peerId;
            var tagGo = new GameObject("nametag");
            tagGo.transform.SetParent(_rig.transform, false);
            tagGo.transform.localPosition = new Vector3(0, 1.4f, 0);
            _tag = tagGo.AddComponent<TextMesh>();
            _tag.text = name; _tag.characterSize = 0.15f; _tag.anchor = TextAnchor.MiddleCenter;
        }

        static bool NewerSeq(ushort a, ushort b) { return (ushort)(a - b) < 32768 && a != b; }

        public void OnSnapshot(PlayerSnapshot s)
        {
            if (_any && !NewerSeq(s.Seq, _lastSeq)) return; // stale: latest-wins
            _lastSeq = s.Seq; _any = true;
            _buf.Add(new Entry { T = Time.time, S = s });
            if (_buf.Count > 32) _buf.RemoveAt(0);
        }

        public void Tick()
        {
            if (_buf.Count == 0 || _rig == null) return;
            float renderT = Time.time - InterpDelay;
            // find bracketing pair
            Entry a = _buf[0], b = _buf[_buf.Count - 1];
            for (int i = _buf.Count - 1; i > 0; i--)
                if (_buf[i - 1].T <= renderT && _buf[i].T >= renderT) { a = _buf[i - 1]; b = _buf[i]; break; }

            Vector3 pa = new Vector3(a.S.PosX, a.S.PosY, a.S.PosZ);
            Vector3 pb = new Vector3(b.S.PosX, b.S.PosY, b.S.PosZ);
            Vector3 pos; float yaw;
            if (renderT >= b.T) // newest too old: extrapolate, capped
            {
                float over = Mathf.Min(renderT - b.T, ExtrapCap);
                float dt = Mathf.Max(b.T - a.T, 0.001f);
                pos = pb + (pb - pa) / dt * over;
                yaw = b.S.Yaw;
            }
            else
            {
                float k = Mathf.InverseLerp(a.T, b.T, renderT);
                pos = Vector3.Lerp(pa, pb, k);
                yaw = Mathf.LerpAngle(a.S.Yaw, b.S.Yaw, k);
            }
            _rig.transform.position = pos;
            _rig.transform.rotation = Quaternion.Euler(0, yaw, 0);
            // stance: squash capsule toward ground (stand=140 -> 1.0, prone=30 -> ~0.2)
            float sy = Mathf.Clamp(b.S.Stance / 140f, 0.2f, 1f);
            _rig.transform.localScale = new Vector3(1, sy, 1);
            // replay delay: age of the snapshot pair being rendered (wrap-safe uint delta)
            ReplayDelayMs = (uint)(Clock.NowMs() - b.S.SentAtMs);
            if (Mathf.Abs(ReplayDelayMs - _shownDelayMs) > 15f) // avoid per-frame TextMesh churn
            {
                _shownDelayMs = ReplayDelayMs;
                _tag.text = _name + " " + Mathf.RoundToInt(ReplayDelayMs) + "ms";
            }
            // billboard nametag
            var cam = Camera.main;
            if (cam != null) _tag.transform.rotation = Quaternion.LookRotation(_tag.transform.position - cam.transform.position);
        }

        public void Destroy() { if (_rig != null) Object.Destroy(_rig); }
    }
}
