using Client;
using networkprotocol;
using Prometheus.Shared.Utils;
using UnityEngine;

namespace Prometheus.Shared.Data
{
    /// <summary>
    /// Ball state and network object, which keeps direction, center, scale, pace and radius. 
    /// </summary>
    public class Ball : Message, IDeepCopy<Ball>
    {
        public Vector2 direction;
        public Vector3 center;
        public float   scale;
        public float   pace;
        public float   radius;

        public Ball() : base(false)
        {
            Type      = (int) GameMessageType.Ball;
            center    = Constants.BallStartPos;
            scale     = Constants.DefaultBallScale;
            radius    = Constants.BallRadius;
            pace      = Constants.BallPace;
            direction = Vector2.zero;
        }

        public override bool Serialize(BaseStream stream)
        {
            stream.serialize_float(ref direction.x);
            stream.serialize_float(ref direction.y);
            stream.serialize_float(ref center.x);
            stream.serialize_float(ref center.y);
            stream.serialize_float(ref center.z);
            stream.serialize_float(ref pace);
            stream.serialize_float(ref scale);
            stream.serialize_float(ref radius);

            return true;
        }

        public Ball Copy()
        {
            return new Ball
            {
                Type      = Type,
                direction = direction,
                center    = center,
                pace      = pace,
                scale     = scale,
                radius    = radius
            };
        }
    }
}