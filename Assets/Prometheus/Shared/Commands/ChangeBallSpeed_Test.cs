using Client;
using networkprotocol;

namespace Prometheus.Shared.Commands
{
    /// <summary>
    /// Test Command class which changes ball speed. 
    /// </summary>
    public class ChangeBallSpeed_Test : Message
    {
        public float delta;

        public ChangeBallSpeed_Test()
        {
            Type  = (int) GameMessageType.ChangeBallSpeed;
            delta = 0;
        }


        public override bool Serialize(BaseStream stream)
        {
            stream.serialize_float(ref delta);

            return true;
        }
    }
}