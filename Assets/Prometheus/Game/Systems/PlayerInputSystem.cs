using Client;
using Prometheus.Game.Components;
using Prometheus.Game.Groups;
using Prometheus.Shared.Commands;
using Prometheus.Shared.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// This system listens player input and processes them such as
    /// Moving or Shooting ball. 
    /// </summary>
    [UpdateInGroup(typeof(InputGroup))]
    public class PlayerInputSystem : ComponentSystem
    {
        private bool    _canDrag;      // Drag flag
        private Vector3 _mouseDownPos; // Last Mouse Down Pos

        protected override void OnUpdate()
        {
            // If game is paused we dont need to continue.
            if (GameManager.Instance.isPaused)
            {
                return;
            }

            CheckDrag();

            Drag();

            Shoot();
        }

        /// <summary>
        /// Checks player can shoot the ball or not.
        /// </summary>
        private void Shoot()
        {
            // If we are in drag state and we released mouse left button.
            if (_canDrag && Input.GetMouseButtonUp(0))
            {
                // Deactivate drag state.
                _canDrag = false;
                Entities.WithAll<PlayerComp>().ForEach((Entity entity, ref PlayerInfoComp infoComp, ref State state) =>
                {
                    // If you are not connected dont process.
                    if (!infoComp.IsConnected)
                    {
                        return;
                    }

                    // Get direction vector.
                    var directionVec = (Input.mousePosition - _mouseDownPos).normalized;
                    directionVec[0] *= -1;

                    // TODO: ERDINC Test

#region Test

                    directionVec = Vector2.up;
                    if (!infoComp.IsOwner)
                    {
                        directionVec *= -1;
                    }

#endregion


                    // Send it to server
                    var playerHit = GameClient.Instance.CreateMessage<PlayerHit>(GameMessageType.PlayerHit);
                    playerHit.PlayerId  = infoComp.Id;
                    playerHit.StateId   = state.Id + 1;
                    playerHit.Direction = directionVec;

                    // Since shooting is really important, we are sending it reliable.
                    GameClient.Instance.SendMessage(GameChannelType.Reliable, playerHit);
                });
            }
        }

        private void Drag()
        {
            // If we are already in drag state and we are still pressing mouse left.
            // So we can drag our object.
            if (_canDrag && Input.GetMouseButton(0))
            {
                // First we move our player respect to our mouse position and
                // We need to store that positions in a fixed size buffer
                // Since we are developing our game with ECS and ECS's main purpose is
                // Fetching data without cache miss and organize them with linear order,
                // in order to do that we need to know the size of our objects.
                // Just like old C times haa reallocs mallocs. :)
                Entities.WithAll<PlayerComp>().ForEach(
                    (Entity entity, ref Translation translation, ref PlayerInfoComp infoComp, ref MoveState moveState) =>
                    {
                        if (Camera.main != null)
                        {
                            // Get mouse point
                            var mouseInput      = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                            // Convert into screen point to calculate with entity translation.
                            var mouseScreenPos  = Camera.main.ScreenToWorldPoint(mouseInput);
                            // Convert it to float3
                            var mouseFloat3Pos  = new float3(mouseScreenPos.x, mouseScreenPos.y, translation.Value.z);
                            // Calculate the difference between mouse and player pos
                            var distanceSquared = math.distancesq(translation.Value, mouseFloat3Pos);

                            // If they are so close we dont need to process it.
                            // This will decrease our process time.
                            if (distanceSquared >= 0.0001f)
                            {
                                // Assign it.
                                translation.Value = mouseFloat3Pos;
                                // Increase state
                                moveState.State++;
                                // Cache it into buffer to check it later.
                                var buffer = ECSManager.Instance.EntityManager.GetBuffer<PlayerPosHistory>(entity);
                                
                                // If we exceed threshold remove the least important point which is first point.
                                while (buffer.Length >= Constants.TotalPlayerMoveHistory)
                                {
                                    buffer.RemoveAt(0);
                                }

                                buffer.Add(new PlayerPosHistory {Id = moveState.State, Pos = translation.Value});

                                // Send it to server.
                                var playerMove = GameClient.Instance.CreateMessage<PlayerMove>(GameMessageType.PlayerMove);
                                playerMove.playerId = infoComp.Id;
                                playerMove.stateId  = moveState.State;
                                playerMove.center   = translation.Value;

                                // Send it with unreliable because, we are sending our position
                                // Quite frankly so that we dont care if some packages missing or not.
                                // That does not change so much thing.
                                GameClient.Instance.SendMessage(GameChannelType.UnReliable, playerMove);
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Checks mouse intersects with player bounds or not.
        /// </summary>
        private void CheckDrag()
        {
            if (!_canDrag && Input.GetMouseButtonDown(0))
            {
                // WorldRenderBounds gives us real size of player sprite.
                Entities.WithAll<PlayerComp>().ForEach(
                    (Entity entity, ref Translation translation, ref WorldRenderBounds worldRenderBounds, ref PlayerInfoComp infoComp) =>
                    {
                        if (Camera.main != null)
                        {
                            // Get mouse screen pos
                            var mouseInput = new Vector3(Input.mousePosition.x, Input.mousePosition.y, translation.Value.z);
                            var pos        = Camera.main.ScreenToWorldPoint(mouseInput);

                            // Assign mouse z with player, because `Bounds.Contains` function checks z as well
                            // Since we are developing 2D game so we dont need to check z coordinate.
                            pos[2]   = translation.Value.z;
                            _canDrag = worldRenderBounds.Value.ToBounds().Contains(pos);
                        }
                    });
                _mouseDownPos = Input.mousePosition;
            }
        }
    }
}