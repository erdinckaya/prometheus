using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Unity.Entities;
using Unity.Transforms;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// This system has only one job which is assign z coordinate, render order.
    /// If z is bigger than other translations z it will be rendered after.
    /// This means that it will be demonstrated under.
    /// </summary>
    [UpdateInGroup(typeof(MoveGroup))]
    public class RenderSortingOrderSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<RenderSortingOrderComp>()
                    .ForEach((Entity entity, ref RenderSortingOrderComp spriteSortingOrderComp, ref Translation translation) =>
                    {
                        translation.Value.z = spriteSortingOrderComp.Value;
                    });
        }
    }
}