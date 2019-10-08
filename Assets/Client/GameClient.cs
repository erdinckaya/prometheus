using System;
using System.Diagnostics;
using networkprotocol;
using Prometheus.Shared.Utils;
using Debug = UnityEngine.Debug;

namespace Client
{
    public class GameClient
    {
        
        private static GameClient _instance;

        /// <summary>
        /// Singleton instance for game client.
        /// </summary>
        public static GameClient Instance => _instance ?? (_instance = new GameClient());


        private          double                 _time;     // Elapsed time in terms of seconds.
        private readonly Stopwatch              _watch;    // Time measure
        private volatile bool                   _running;  // Running flag for main client thread loop.
        private readonly networkprotocol.Client _client;   // Yojimbo Client instance
        private readonly GameConnectionConfig   _config;   // Basic game config
        private          ulong                  _clientId; // Generated Client id
        private          Action<Message>        _action;   // Message callback to notify Unity main thread.


        private GameClient()
        {
            // Initializing variables and configs 
            _time    = 0;
            _running = true;
            _config  = new GameConnectionConfig();

            var adapter = new GameAdapter(this);
            var address = new Address("0.0.0.0");
            _client = new networkprotocol.Client(yojimbo.DefaultAllocator, address, _config, adapter, _time);

            _watch = new Stopwatch();

            // Setting yojimbo logger level and binding it to Unity debug log.
            yojimbo.log_level(yojimbo.LOG_LEVEL_INFO);
            yojimbo.set_printf_function(Debug.Log);
        }

        /// <summary>
        /// Creates network objects with given type. Note that network objects must have
        /// default constructors and must be registered to `GameMessageFactory`.
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateMessage<T>(GameMessageType type) where T : Message
        {
            return _client.CreateMessage((int) type) as T;
        }


        /// <summary>
        /// Sets package loss for debugging
        /// </summary>
        /// <param name="percentage">0 to 100 percentage</param>
        public void SetPackageLost(float percentage)
        {
            _client.SetPacketLoss(percentage);
        }

        /// <summary>
        /// Sets latency for debugging
        /// </summary>
        /// <param name="milliseconds">Milliseconds</param>
        public void SetLatency(float milliseconds)
        {
            _client.SetLatency(milliseconds);
        }

        /// <summary>
        /// Sets message callback to notify Unity.
        /// </summary>
        /// <param name="action"></param>
        public void SetCallback(Action<Message> action)
        {
            _action = action;
        }

        /// <summary>
        /// Starts client. First connects to server then runs its game loop.
        /// </summary>
        public void Start()
        {
            Connect();
            Run();
        }

        /// <summary>
        /// Disconnects from server and closes its connections.
        /// </summary>
        public void Quit()
        {
            _running = false;
        }

        /// <summary>
        /// Connects to server with insecure way. Note that, if you want to connect to server with
        /// secure way you need to implement your matcher system. I have already implemented it in 
        /// `ClientMatcher` class but, it uses `mbed_tls` library which is not implemented or inhereted
        /// from original C++ yojimbo. Therefore if you want to connect with secure way you need to encript
        /// your data and use some matchmaking server to handle it. For now we will use insecure connection
        /// with just protocol id and empty private key. 
        /// </summary>
        private void Connect()
        {
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, "connecting client (insecure)");

            // Since we use insecure connection we need to create a client id. 
            yojimbo.random_bytes(ref _clientId, 8);
            var serverAddress = new Address(Constants.ServerAddress, Constants.ServerPort);
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"connecting to server {serverAddress}");

            // Create an insecure connection with protocol id and empty private key.
            // Yojimbo will generate new token for us automaticly.
            var privateKey = new byte[yojimbo.KeyBytes];
            _client.InsecureConnect(privateKey, _clientId, serverAddress);

            var addressString = _client.Address.ToString();
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"client address is {addressString}");
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"client id is {_clientId}");
        }

        /// <summary>
        /// Runs client loop.
        /// </summary>
        private void Run()
        {
            // This functions is the main part of client network loop.

            // Start watch for calculating elapsed time in terms of seconds. Because yojimbo works second metric.
            _watch.Start();
            _time = 0;
            // This loop runs every `Constants.DeltaTime` for syncing client and server time.
            while (_running)
            {
                double elapsedTime = _watch.Elapsed.TotalSeconds;
                if (_time <= elapsedTime)
                {
                    // This part is the most import part, you need to execute
                    // your time and message processes with that exact order.
                    // 1 -> AdvanceTime
                    // 2 -> Receive Packages
                    // 3 -> Process Messages
                    // 4 -> Send Packages

                    _client.AdvanceTime(_time);
                    _time += Constants.DeltaTime;

                    _client.ReceivePackets();

                    ProcessMessages();

                    if (_client.IsDisconnected)
                        break;


                    if (_client.ConnectionFailed)
                        break;

                    _client.SendPackets();
                }
                else
                {
                    // Sleep till next tick time.
                    yojimbo.sleep(_time - elapsedTime);
                }
            }

            // Stop the client if anything goes wrong.
            _watch.Stop();
            _client.Disconnect();
        }


        /// <summary>
        /// Processes messages which they can come either reliable or unreliable channel.
        /// </summary>
        private void ProcessMessages()
        {
            if (_client.IsConnected)
            {
                for (int i = 0; i < _config.numChannels; i++)
                {
                    var message = _client.ReceiveMessage(i);

                    while (message != null)
                    {
                        ProcessMessage((GameChannelType) i, message);
                        _client.ReleaseMessage(ref message);
                        message = _client.ReceiveMessage(i);
                    }
                }
            }
        }

        /// <summary>
        /// Notifies game logic with new coming messages.
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="message"></param>
        private void ProcessMessage(GameChannelType channelType, Message message)
        {
            _action?.Invoke(message);
        }

        /// <summary>
        /// Returns an object that includes network objects,
        /// such as RTT, package loss, bandwidth etc. 
        /// </summary>
        /// <returns></returns>
        public NetworkInfo GetNetworkInfo()
        {
            NetworkInfo info;
            _client.GetNetworkInfo(out info);
            return info;
        }

        /// <summary>
        /// Returns elapsed time in terms of second.
        /// </summary>
        public double Time => _watch.Elapsed.TotalSeconds;

        /// <summary>
        /// Returns client id.
        /// </summary>
        public ulong ClientId => _clientId;

        /// <summary>
        /// Send network message to server
        /// </summary>
        /// <param name="channelType">Reliable or Unreliable. Note: `GameChannelType`</param>
        /// <param name="message"></param>
        public void SendMessage(GameChannelType channelType, Message message)
        {
            if (_client.IsConnected)
            {
                _client.SendMessage((int) channelType, message);
            }
        }

        /// <summary>
        /// Releases messages from message factory pool. This function needs to be called
        /// when we are done with messages to clean them all.
        /// </summary>
        /// <param name="message"></param>
        public void ReleaseMessage(Message message)
        {
            _client?.ReleaseMessage(ref message);
        }

        /// <summary>
        /// Client connection successful callback
        /// </summary>
        /// <param name="clientIndex"></param>
        public void OnServerClientConnected(int clientIndex)
        {
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"Client has connected to server with id {clientIndex}");
        }

        /// <summary>
        /// Client connection unsuccessful callback
        /// </summary>
        /// <param name="clientIndex"></param>
        public void OnServerClientDisconnected(int clientIndex)
        {
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, $"Client has disconnected to server with id {clientIndex}");
        }
    }
}