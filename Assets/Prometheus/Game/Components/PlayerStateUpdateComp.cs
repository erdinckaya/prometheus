using Unity.Entities;
using Unity.Mathematics;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// PlayerStateUpdateComp is equivalent of ball network object
    /// to pass its data to ECS 
    /// </summary>
    public struct PlayerStateUpdateComp : IComponentData
    {
        public ulong Id;
        public ulong MoveState;
        public ulong State;
        public float3 Pos;
        public float Scale;
        public bool IsOwner;
    }
}