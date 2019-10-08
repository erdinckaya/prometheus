using networkprotocol;
using Prometheus.Shared.Commands;
using Prometheus.Shared.Data;

namespace Client
{
    /// <summary>
    /// You should register your network objects in this class.
    /// Notice that your client and server's message factory must be same!
    /// </summary>
    internal class GameMessageFactory : MESSAGE_FACTORY_START
    {
        public GameMessageFactory(Allocator allocator) : base(allocator, (int) GameMessageType.Count)
        {
            DECLARE_MESSAGE_TYPE((int) GameMessageType.PlayerMove, typeof(PlayerMove));
            DECLARE_MESSAGE_TYPE((int) GameMessageType.Ball, typeof(Ball));
            DECLARE_MESSAGE_TYPE((int) GameMessageType.Player, typeof(Player));
            DECLARE_MESSAGE_TYPE((int) GameMessageType.GameState, typeof(GameState));
            DECLARE_MESSAGE_TYPE((int) GameMessageType.PlayerHit, typeof(PlayerHit));
            DECLARE_MESSAGE_TYPE((int) GameMessageType.Reset, typeof(ResetCommand));
            DECLARE_MESSAGE_TYPE((int) GameMessageType.Pause, typeof(PauseCommand));


#region Test

            DECLARE_MESSAGE_TYPE((int) GameMessageType.ChangeBallSpeed, typeof(ChangeBallSpeed_Test));

#endregion

            MESSAGE_FACTORY_FINISH();
        }
    }
}