using Client;
using networkprotocol;

namespace Prometheus.Shared.Data
{
    /// <summary>
    /// State of the game in one tick. Keeps the game worlds state to notify clients
    /// and make necessary calculations in terms of back and forth.
    /// </summary>
    public class GameState : Message, IDeepCopy<GameState>
    {
        public Player[] Players;
        public Ball     Ball;
        public ulong    StateId;
        public ulong    Owner;

        public GameState() : base(false)
        {
            Reset();
        }

        public override bool Serialize(BaseStream stream)
        {
            Ball.Serialize(stream);
            for (var i = 0; i < Players.Length; i++)
            {
                Players[i].Serialize(stream);
            }

            stream.serialize_uint64(ref StateId);
            stream.serialize_uint64(ref Owner);

            return true;
        }

        /// <summary>
        /// Resets all state to origin.
        /// </summary>
        public void Reset()
        {
            Type    = (int) GameMessageType.GameState;
            StateId = 0;
            Owner   = 0;
            Players = new Player[2];
            for (var index = 0; index < Players.Length; index++)
            {
                Players[index] = new Player((ulong) index, index == 0);
            }

            Ball = new Ball();
        }

        /// <summary>
        /// Adds player to state.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientIndex"></param>
        public void AddPlayer(ulong clientId, int clientIndex)
        {
            if (clientIndex < Players.Length)
            {
                Players[clientIndex] = new Player(clientId, clientIndex == 0);
                if (clientIndex == 1)
                {
                    Owner = Players[0].playerId;
                }
            }
        }

        /// <summary>
        /// Casual copy function
        /// </summary>
        /// <returns></returns>
        public GameState Copy()
        {
            var gs = new GameState {StateId = StateId, Ball = Ball.Copy(), Players = new Player[Players.Length]};
            for (var i = 0; i < gs.Players.Length; i++)
            {
                gs.Players[i] = Players[i].Copy();
            }

            return gs;
        }
    }
}