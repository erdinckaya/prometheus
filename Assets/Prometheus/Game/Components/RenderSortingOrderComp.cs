using Unity.Entities;

namespace Prometheus.Game.Components
{
    /// <summary>
    /// Keeps render order of RenderMesh objects which is z value of its
    /// Translation components.
    /// </summary>
    public struct RenderSortingOrderComp : IComponentData
    {
        public float Value;
    }
}