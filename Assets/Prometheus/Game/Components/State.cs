using Unity.Entities;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// Keeps objects states.
    /// </summary>
    public struct State : IComponentData
    {
        public ulong Id;
    }
}