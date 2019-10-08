using Unity.Entities;
using Unity.Mathematics;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// BallStateUpdateComp is equivalent of ball network object
    /// to pass its data to ECS 
    /// </summary>
    public struct BallStateUpdateComp : IComponentData
    {
        public float2 Direction;
        public float3 Center;
        public float  Scale;
        public float  Pace;
        public ulong  State;
    }
}