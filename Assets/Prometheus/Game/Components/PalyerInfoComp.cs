using Unity.Entities;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// Keeps player's basic information.
    /// </summary>
    public struct PlayerInfoComp : IComponentData
    {
        public ulong Id;
        public bool  IsOwner;
        public bool  IsConnected;
    }
}