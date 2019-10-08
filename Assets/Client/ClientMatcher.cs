using networkprotocol;
using Prometheus.Shared.Utils;

namespace Client
{
    /// <summary>
    /// This Class creates connection to match maker server and get client id and server info.
    /// Matcher works only secure connections. Since Yojimbo C# repo didnt implement MBED_TLS
    /// We dont have any secure connections. Nevertheless you can write your own matcher and
    /// Own match making server. I am leaving this class as an example.
    /// </summary>
    public class ClientMatcher
    {
        // Yojimbo Matcher
        private readonly Matcher _matcher;
        private readonly byte[]  _connectToken;
        private readonly ulong   _clientId;


        public ClientMatcher()
        {
            _matcher      = new Matcher(yojimbo.DefaultAllocator);
            _clientId     = 0;
            _connectToken = new byte[yojimbo.ConnectTokenBytes];
            yojimbo.random_bytes(ref _clientId, 8);
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"client id is {_clientId:x16}\n");
        }

        public bool RequestMatch()
        {
            if (!_matcher.Initialize())
            {
                yojimbo.printf(yojimbo.LOG_LEVEL_ERROR, $"error Failed to initialize matcher!\n");
            }

            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"requesting match from https://localhost:8080\n");

            _matcher.RequestMatch(Constants.ProtocolId, _clientId, false);

            if (_matcher.MatchStatus == MatchStatus.MATCH_FAILED)
            {
                yojimbo.printf(yojimbo.LOG_LEVEL_ERROR,
                    "\nRequest match failed. Is the matcher running? Please run \"premake5 matcher\" before you connect a secure client\n");
                return false;
            }


            _matcher.GetConnectToken(_connectToken);
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, "received connect token from matcher\n");
            return true;
        }

        public byte[] ConnectToken => _connectToken;

        public ulong ClientId => _clientId;
    }
}