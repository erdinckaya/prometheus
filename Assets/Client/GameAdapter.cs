using networkprotocol;

namespace Client
{
    /// <summary>
    /// Game Adapter which notifies client connections.
    /// </summary>
    public class GameAdapter : Adapter
    {
        private readonly GameClient _client;

        public GameAdapter(GameClient client)
        {
            _client = client;
        }
        
        /// <summary>
        /// Creates Message factory with given allocator.
        /// </summary>
        /// <param name="allocator">Usually it is yojimbo default allocator.</param>
        /// <returns></returns>
        public override MessageFactory CreateMessageFactory(Allocator allocator) 
        {
            return new GameMessageFactory(allocator);
        }

        /// <summary>
        /// Client connect event dispatcher.
        /// </summary>
        /// <param name="clientIndex"></param>
        public override void OnServerClientConnected(int clientIndex)
        {
            _client?.OnServerClientConnected(clientIndex);
        }

        /// <summary>
        /// Client disconnect event dispatcher.
        /// </summary>
        /// <param name="clientIndex"></param>
        public override void OnServerClientDisconnected(int clientIndex)
        {
            _client?.OnServerClientDisconnected(clientIndex);
        }
    }
}