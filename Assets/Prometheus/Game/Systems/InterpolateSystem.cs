using System;
using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// Interpolate System is basic system. It has only one job, interpolating one
    /// Translation one point to another point with given duration. It is basically
    /// linear interpolation. In future you can add Easing functions as well.
    /// </summary>
    [UpdateInGroup(typeof(InterpolateGroup))]
    public class InterpolateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (GameManager.Instance.isPaused)
            {
                return;
            }

            Entities.WithAll<InterpolateComp>().ForEach((Entity entity, ref InterpolateComp interpolateComp, ref Translation translation) =>
            {
                // If duration is zero we dont need to calculate.
                if (Math.Abs(interpolateComp.Duration) < 0.0001f)
                {
                    return;
                }
                
                // Calculate progress of interpolation.
                interpolateComp.Progress += Time.deltaTime / interpolateComp.Duration;
                // Clamp it with 1.0f, because we dont want to go further.
                if (interpolateComp.Progress > 0.9f)
                {
                    interpolateComp.Progress = 1.0f;
                }

                // Assign it.
                var delta = (interpolateComp.End - interpolateComp.Start) * interpolateComp.Progress;
                translation.Value = interpolateComp.Start + delta;
            });
        }
    }
}