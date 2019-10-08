using Client;
using networkprotocol;
using UnityEngine;

namespace Prometheus.Shared.Commands
{
    /// <summary>
    /// Player Move commands is dispatched by only Client. Client sends players positions
    /// via unreliable channel and waits for server correction. Since this game has not
    /// any obstacles or whatever you prevent from moving through or across, server only
    /// adds this movements to state without any check and it sends this position in next state update.
    /// </summary>
    public class PlayerMove : Message
    {
        public ulong  stateId;
        public ulong  playerId;
        public Vector3 center;

        public PlayerMove()
        {
            Type = (int) GameMessageType.PlayerMove;

            playerId = 0;
            stateId  = 0;
            center   = Vector3.zero;
        }

        public override bool Serialize(BaseStream stream)
        {
            stream.serialize_uint64(ref playerId);
            stream.serialize_uint64(ref stateId);
            stream.serialize_float(ref center.x);
            stream.serialize_float(ref center.y);
            stream.serialize_float(ref center.z);

            return true;
        }
    }
}