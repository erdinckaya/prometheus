using Client;
using Unity.Mathematics;
using UnityEngine;

namespace Prometheus.Game.Debug
{
    /// <summary>
    /// Basic IMGui MonoBehaviour to see values on screen.
    /// </summary>
    public class OnGuiDebugger : MonoBehaviour
    {
        public static OnGuiDebugger Instance;

        public float3 PlayerPos;
        public float3 RivalPos;
        public float3 BallPos;
        public float  BallPace;

        private void Awake()
        {
            Instance = this;
        }


        private void OnGUI()
        {
            var offset = 10;
            var diff   = 20;

            var style       = new GUIStyle {fontSize = 20, normal = {textColor = Color.white}};
            var networkInfo = GameClient.Instance.GetNetworkInfo();


            GUI.Label(new Rect(10, offset, 100, 100), $"RTT = {networkInfo.RTT}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Acked Bandwitdh = {networkInfo.ackedBandwidth}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Received Bandwitdh = {networkInfo.receivedBandwidth}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Sent Bandwitdh = {networkInfo.sentBandwidth}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Package Loss = {networkInfo.packetLoss}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Ball speed = {BallPace}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Ball pos = {BallPos}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Player1 center = {PlayerPos}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Player2 center = {RivalPos}", style);
            offset += diff;
            GUI.Label(new Rect(10, offset, 100, 100), $"Pause = {GameManager.Instance.isPaused}", style);
        }
    }
}