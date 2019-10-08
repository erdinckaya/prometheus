using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Prometheus.Shared.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// This system dispatches the interpolations by fetching them from history.
    /// </summary>
    [UpdateInGroup(typeof(MoveGroup))]
    public class RivalMoveSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (GameManager.Instance.isPaused)
            {
                return;
            }

            Entities.WithAll<RivalComp>().ForEach((Entity entity, ref Translation translation, ref InterpolateComp interpolateComp) =>
            {
                var history = ECSManager.Instance.EntityManager.GetBuffer<PlayerPosHistory>(entity);
                if (history.Length < 1)
                {
                    return;
                }

                // Execute if last interpolation is done or it is just a beginning of interpolation.
                if (interpolateComp.Progress >= 0.9f || math.abs(interpolateComp.Progress) <= 0.0001f)
                {
                    interpolateComp.Start    = translation.Value;
                    interpolateComp.End      = history[0].Pos;
                    interpolateComp.Duration = (float) Constants.DeltaTime;
                    interpolateComp.Progress = 0;
                    history.RemoveAt(0);
                }
            });
        }
    }
}