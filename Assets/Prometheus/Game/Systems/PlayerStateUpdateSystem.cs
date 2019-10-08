using System.Linq;
using Client;
using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Prometheus.Shared.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// This system is responsible from passing server state to client.
    /// </summary>
    [UpdateInGroup(typeof(NetworkGroup))]
    public class PlayerStateUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref PlayerStateUpdateComp playerStateUpdateComp) =>
            {
                UpdatePlayers(playerStateUpdateComp);
                PostUpdateCommands.DestroyEntity(entity);
            });
        }

        private void UpdatePlayers(PlayerStateUpdateComp player)
        {
            // If state belongs to us, we need to update `PlayerComp`.
            if (player.Id == GameClient.Instance.ClientId)
            {
                Entities.WithAll<PlayerComp>().ForEach((Entity       entity, ref Translation translation,
                                                        ref State    state,  ref Scale       scale, ref MoveState moveState,
                                                        ref PlayerInfoComp infoComp) =>
                {
                    // Update player components.
                    scale.Value          = player.Scale;
                    moveState.State      = player.MoveState;
                    state.Id             = player.State;
                    infoComp.Id          = player.Id;
                    infoComp.IsOwner     = player.IsOwner;
                    infoComp.IsConnected = true;

                    // Check player's position history.
                    var history = ECSManager.Instance.EntityManager.GetBuffer<PlayerPosHistory>(entity);
                    while (history.Length >= Constants.TotalPlayerMoveHistory)
                    {
                        history.RemoveAt(0);
                    }

                    var data = new PlayerPosHistory {Id = moveState.State, Pos = player.Pos};
                    if (history.Length != 0)
                    {
                        // Get server position and compare it with client position in same state.
                        const float tolerance = 0.001f;
                        while (history.Length != 0)
                        {
                            var h = history[0];
                            // Find state.
                            if (h.Id == data.Id)
                            {
                                var distance = math.distance(h.Pos, data.Pos);
                                // Check distance if distance is really huge, that means 
                                // Server does not allow you to go there so that we need to obey
                                // Server and correct our position respect to server state.
                                if (distance > tolerance)
                                {
                                    history.Clear();
                                    history.Add(data);
                                    translation.Value = player.Pos;
                                }
                                break;
                            }
                            // If there are past state we dont need to keep them.
                            history.RemoveAt(0);
                        }
                    }
                    else
                    {
                        history.Add(data);
                        translation.Value = player.Pos;
                    }
                });
            }
            else
            {
                // Update rival components.
                Entities.WithAll<RivalComp>().ForEach((Entity       entity, ref Translation translation,
                                                       ref State    state,  ref Scale       scale, ref MoveState moveState,
                                                       ref PlayerInfoComp infoComp) =>
                {
                    scale.Value          = player.Scale;
                    moveState.State      = player.MoveState;
                    state.Id             = player.State;
                    infoComp.Id          = player.Id;
                    infoComp.IsOwner     = player.IsOwner;
                    infoComp.IsConnected = true;


                    // Just add server positions in to buffer, after that we will interpolate them later in different system.
                    var distance = math.distance(translation.Value, player.Pos);
                    if (distance >= 0.001f)
                    {
                        var history = ECSManager.Instance.EntityManager.GetBuffer<PlayerPosHistory>(entity);
                        while (history.Length >= Constants.TotalPlayerMoveHistory)
                        {
                            history.RemoveAt(0);
                        }

                        var data = new PlayerPosHistory {Id = moveState.State, Pos = player.Pos};
                        history.Add(data);
                    }
                });
            }
        }
    }
}