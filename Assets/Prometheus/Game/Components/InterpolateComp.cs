using Unity.Entities;
using Unity.Mathematics;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// Interpolation component between to point with duration
    /// Basic linear interpolation.
    /// </summary>
    public struct InterpolateComp : IComponentData
    {
        public float3 Start;
        public float3 End;
        public float  Duration;
        public float  Progress;
    }
}