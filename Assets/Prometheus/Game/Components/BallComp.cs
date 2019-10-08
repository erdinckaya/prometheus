using Unity.Entities;
using Unity.Mathematics;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// Ball component keeps pace, direction, and its state 
    /// </summary>
    public struct BallComp : IComponentData
    {
        public float  Pace;
        public ulong  State;
        public float2 Direction;
    }
}