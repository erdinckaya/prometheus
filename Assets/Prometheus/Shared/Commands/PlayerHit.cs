using Client;
using networkprotocol;
using UnityEngine;

namespace Prometheus.Shared.Commands
{
    /// <summary>
    /// Player Hit command is dispatched from Client only. Server checks hit state
    /// and re-simulate the world to decide whether player can hit or not.
    /// </summary>
    public class PlayerHit : Message
    {
        public ulong   PlayerId;
        public ulong   StateId;
        public Vector2 Direction;

        public PlayerHit() : base(false)
        {
            Type = (int) GameMessageType.PlayerHit;
        }

        public override bool Serialize(BaseStream stream)
        {
            stream.serialize_uint64(ref PlayerId);
            stream.serialize_uint64(ref StateId);
            stream.serialize_float(ref Direction.x);
            stream.serialize_float(ref Direction.y);
            return true;
        }
    }
}