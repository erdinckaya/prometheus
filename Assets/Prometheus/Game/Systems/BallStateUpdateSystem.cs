using System;
using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Prometheus.Shared.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// This system updates ball's features according to received the most recent server state.
    /// Checks ball's position whether it is too far from server state or not. If it is far from
    /// It corrects ball's position immediately, otherwise it does not care
    /// </summary>
    [UpdateInGroup(typeof(NetworkGroup))]
    public class BallStateUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<BallStateUpdateComp>().ForEach((Entity entity, ref BallStateUpdateComp ballStateUpdateComp) =>
            {
                UpdateBall(ballStateUpdateComp);
                PostUpdateCommands.DestroyEntity(entity);
            });
        }

        private void UpdateBall(BallStateUpdateComp ballState)
        {
            // Fetches ball components. 
            Entities.ForEach(
                (Entity entity, ref Translation translation, ref Scale scale, ref BallComp ballComp, ref InterpolateComp interpolateComp) =>
                {
                    ballComp.Direction = ballState.Direction;
                    ballComp.Pace      = ballState.Pace;
                    ballComp.State     = ballState.State;
                    scale.Value        = ballState.Scale;


                    var distance = math.distance(translation.Value, ballState.Center);

                    // If ball goes so far away (Approximately 5 state diff for now) we need to correct it!
                    var         maxDelta  = math.length(ballComp.Pace * Constants.MaxBallStateDiff * ballComp.Direction);
                    const float tolerance = 0.001f;
                    // Check if it is staying steady, if it is ignore that check.
                    bool isZeroDirection = Math.Abs(ballComp.Direction.x) < tolerance && Math.Abs(ballComp.Direction.y) < tolerance;
                    if (maxDelta <= distance && (isZeroDirection || maxDelta > 0))
                    {
                        if (!isZeroDirection)
                        {
                            UnityEngine.Debug.Log($"Max distance {maxDelta} real distance {distance}");
                            UnityEngine.Debug.Log($"val {translation.Value} ballstate {ballState.Center}");
                        }

                        translation.Value = ballState.Center;
                    }
                });
        }
    }
}