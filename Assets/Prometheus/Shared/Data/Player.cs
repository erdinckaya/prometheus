using Client;
using networkprotocol;
using Prometheus.Shared.Utils;
using UnityEngine;

namespace Prometheus.Shared.Data
{
    /// <summary>
    /// Player state and network object.
    /// </summary>
    public class Player : Message, IDeepCopy<Player>
    {
        public ulong   playerId;
        public ulong   playerMoveId;
        public Vector3 center;
        public float   scale;
        public float   radius;

        public Player(ulong playerId, bool isFirst) : base(false)
        {
            Type          = (int) GameMessageType.Player;
            this.playerId = playerId;
            center        = isFirst ? Constants.PlayerOneStartPos : Constants.PlayerTwoStartPos;
            scale         = Constants.DefaultPlayerScale;
            radius        = Constants.PlayerRadius;
            playerMoveId  = 0;
        }


        public override bool Serialize(BaseStream stream)
        {
            stream.serialize_float(ref center.x);
            stream.serialize_float(ref center.y);
            stream.serialize_float(ref center.z);
            stream.serialize_float(ref scale);
            stream.serialize_float(ref radius);
            stream.serialize_uint64(ref playerId);
            stream.serialize_uint64(ref playerMoveId);

            return true;
        }

        public Player Copy()
        {
            var p = new Player(playerId, false)
            {
                playerId = playerId, center = center, scale = scale, radius = radius, playerMoveId = playerMoveId
            };
            return p;
        }
    }
}