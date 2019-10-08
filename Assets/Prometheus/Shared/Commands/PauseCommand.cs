using Client;
using networkprotocol;

namespace Prometheus.Shared.Commands
{
    /// <summary>
    /// Pause Command pauses the game. Client sends pause command and server ignores its value
    /// because server only uses its pause value which is `GameManager.IsPaused`
    /// </summary>
    public class PauseCommand : Message
    {
        public bool Value;

        public PauseCommand() : base(false)
        {
            Type = (int) GameMessageType.Pause;
        }
        
        public override bool Serialize(BaseStream stream)
        {
            stream.serialize_bool(ref Value);
            return true;
        }
    }
}