using Client;
using networkprotocol;

namespace Prometheus.Shared.Commands
{
    /// <summary>
    /// Resets the state of game.
    /// </summary>
    public class ResetCommand : Message
    {
        public ResetCommand() : base(false)
        {
            Type = (int) GameMessageType.Reset;
        }
    }
}