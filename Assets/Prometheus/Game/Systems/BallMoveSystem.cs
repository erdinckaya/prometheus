using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Prometheus.Shared.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// Since ball is deterministic object iy should move always unless we stop it.
    /// Therefore this system calculates ball's next position, without server's permission.
    /// If ball passes through forbidden area server must correct it after next tick.
    /// Nevertheless this process does not belong this system. 
    /// </summary>
    [UpdateInGroup(typeof(MoveGroup))]
    public class BallMoveSystem : ComponentSystem
    {
        /// <summary>
        /// Calculate next position and send it to interpolation to interpolate it
        /// </summary>
        protected override void OnUpdate()
        {
            if (GameManager.Instance.isPaused)
            {
                return;
            }

            Entities.ForEach(
                (Entity entity, ref BallComp ballComp, ref InterpolateComp interpolateComp, ref Translation translation) =>
                {
                    // Calculate next position with pace and direction vector.
                    var delta = ballComp.Pace * ballComp.Direction;

                    interpolateComp.Start    = translation.Value;
                    interpolateComp.End      = translation.Value + new float3(delta.x, delta.y, 0);
                    interpolateComp.Duration = (float) Constants.DeltaTime;
                    interpolateComp.Progress = 0;
                    translation.Value = interpolateComp.End;
                });
        }
    }
}