using Prometheus.Shared.Utils;
using Unity.Entities;
using Unity.Mathematics;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// This is a simple fixed size buffer component
    /// When you want to keep dynamic data you can use this kind
    /// of buffering.
    /// </summary>
    [InternalBufferCapacity(Constants.TotalPlayerMoveHistory)]
    public struct PlayerPosHistory : IBufferElementData
    {
        public ulong  Id;
        public float3 Pos;
    }
}