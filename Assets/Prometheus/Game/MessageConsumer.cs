using System.Collections.Generic;
using System.Linq;
using Client;
using networkprotocol;
using Prometheus.Game.Components;
using Prometheus.Shared.Commands;
using Prometheus.Shared.Data;
using UnityEngine;

namespace Prometheus.Game
{
    /// <summary>
    /// This is basic consumer for server messages.
    /// Collects messages and passes them to ECS World.
    /// </summary>
    public class MessageConsumer
    {
        private readonly Queue<Message> _messages;

        public MessageConsumer()
        {
            _messages = new Queue<Message>();
        }


        /// <summary>
        /// Every Server unity Update this function is called, and consumes all messages.
        /// This means it passes to ECS World.
        /// </summary>
        /// <param name="ecsManager"></param>
        public void Update(ECSManager ecsManager)
        {
            GameState state = null;
            while (_messages.Any())
            {
                // Get Message
                var message     = _messages.Dequeue();
                var messageType = (GameMessageType) message.Type;
                switch (messageType)
                {
                    case GameMessageType.GameState:
                        if (state == null)
                        {
                            state = (GameState) message;
                        }
                        else if (state.Id < message.Id)
                        {
                            // Since server streams game state, states can received different order
                            // Ex: [2, 1, 4, 5, 7, 10, 9, 9].
                            // Lets assume numbers are state numbers, and it can be different order 
                            // Or duplicate or missing. Therefore we need to take the most significant one
                            // Which is last state.
                            // In my experience this state array length usually is not bigger than 4.
                            
                            
                            // We are changing our state so that we dont need to keep it just free that object.
                            GameClient.Instance.ReleaseMessage(state);
                            state = (GameState) message;
                        }

                        break;
                    case GameMessageType.Pause:
                        // If pause command comes assign pause flag.
                        GameManager.Instance.isPaused = ((PauseCommand) message).Value;
                        UnityEngine.Debug.Log($"Game is paused {GameManager.Instance.isPaused}");
                        break;
                }
            }

            // If we have state we need to process it, normally we always expecting state.
            // This is just a safety check.
            if (state != null)
            {
                UpdateState(state, ecsManager);
            }
        }

        private void UpdateState(GameState gameState, ECSManager ecsManager)
        {
            // Check game owner, if game owner is other player, reverse the camera
            // So that we dont need to handle converting positions respectively.
            if (Camera.main != null && gameState.Owner != GameClient.Instance.ClientId)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            // If game is not paused, process the state.
            if (!GameManager.Instance.isPaused)
            {
                // Create update entities, these entities are just input entities
                // Namely disposable one time entities.
                foreach (var player in gameState.Players)
                {
                    var entity = ecsManager.EntityManager.CreateEntity(typeof(PlayerStateUpdateComp));
                    ecsManager.EntityManager.SetComponentData(entity, new PlayerStateUpdateComp
                    {
                        Id        = player.playerId,
                        IsOwner   = gameState.Owner == player.playerId,
                        MoveState = player.playerMoveId,
                        Pos       = player.center,
                        Scale     = player.scale,
                        State     = gameState.StateId
                    });
                }

                var ballEntity = ecsManager.EntityManager.CreateEntity(typeof(BallStateUpdateComp));
                ecsManager.EntityManager.SetComponentData(ballEntity, new BallStateUpdateComp
                {
                    State     = gameState.StateId,
                    Center    = gameState.Ball.center,
                    Direction = gameState.Ball.direction,
                    Pace      = gameState.Ball.pace,
                    Scale     = gameState.Ball.scale
                });
            }
            GameClient.Instance.ReleaseMessage(gameState);
        }

        /// <summary>
        /// Adds server messages
        /// </summary>
        /// <param name="message">Server message class which is basic yojimbo network object</param>
        public void AddMessage(Message message)
        {
            _messages.Enqueue(message);
        }
    }
}