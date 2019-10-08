using Unity.Entities;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// Keeps internal move states of individual objects such as ball player.
    /// </summary>
    public struct MoveState : IComponentData
    {
        public ulong State;
    }
}