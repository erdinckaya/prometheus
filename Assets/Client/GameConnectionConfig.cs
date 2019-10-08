using networkprotocol;
using Prometheus.Shared.Utils;

namespace Client
{
    /// <summary>
    /// Basic Connection configs.
    /// </summary>
    public class GameConnectionConfig : ClientServerConfig
    {
        public GameConnectionConfig()
        {
            protocolId                                     = Constants.ProtocolId;
            numChannels                                    = (int) GameChannelType.Count;
            channel[(int) GameChannelType.Reliable].type   = ChannelType.CHANNEL_TYPE_RELIABLE_ORDERED;
            channel[(int) GameChannelType.UnReliable].type = ChannelType.CHANNEL_TYPE_UNRELIABLE_UNORDERED;
        }
    }
}