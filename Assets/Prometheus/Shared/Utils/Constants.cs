using UnityEngine;

namespace Prometheus.Shared.Utils
{
    /// <summary>
    /// Constant shared data between client and server.
    /// </summary>
    public static class Constants
    {
        public const ulong ProtocolId = 0x11223344556677UL;
        public const int   ServerPort = 40001;

        public const double DeltaTime = 1 / 30.0;
        
        public const string ServerAddress = "127.0.0.1";

        public static readonly Vector3 BallStartPos      = new Vector3(0, -3, 0);
        public static readonly Vector3 PlayerOneStartPos = new Vector3(0, -4, 1);
        public static readonly Vector3 PlayerTwoStartPos = new Vector3(0, 4, 1);

        public const float DefaultBallScale   = 0.4f;
        public const float DefaultPlayerScale = 1f;
        public const float BallRadius         = 0.2f;
        public const float PlayerRadius       = 0.5f;
        public const float MaxBallStateDiff   = 5.0f;
        public const float BallPace           = 0.1f;

        public const int TotalPlayerMoveHistory = 20;
    }
}