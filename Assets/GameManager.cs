using System.Threading;
using Client;
using networkprotocol;
using NLog.Unity;
using Prometheus.Game;
using Prometheus.Game.Debug;
using Prometheus.Shared.Commands;
using Prometheus.Shared.Utils;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basically this class is our main class to start the game.
/// </summary>
public class GameManager : MonoBehaviour
{
#region DEBUG_BUTTON_IMPL

    [SerializeField] private Button               ResetButton;
    [SerializeField] private Button               IbsButton;
    [SerializeField] private Button               DbsButton;
    [SerializeField] private Button               AplButton;
    [SerializeField] private Button               DplButton;
    [SerializeField] private Button               AdlButton;
    [SerializeField] private Button               DdlButton;
    [SerializeField] private Button               PauseButton;
    

#endregion


#region DEBUG_UTILS

    [SerializeField] private ClientSocketAppender LogAdapter;
    [SerializeField] private bool                 EnableNLog;

    private float _testLatency;
    private float _testPackageLoss;

#endregion

    private readonly ECSManager      _ecsManager      = new ECSManager();      // ECSManager for ECS
    private readonly MessageConsumer _messageConsumer = new MessageConsumer(); // Network message consumer.
    public           bool            isPaused;                                 // Static Pause Flag
    private          GameClient      _client;                                  // Yojimbo Game Client

    public static GameManager Instance { get; private set; } // Singleton instance.


    private void OnDestroy()
    {
#region REMOVE_DEBUG_BUTTONS

        ResetButton.onClick.RemoveAllListeners();
        IbsButton.onClick.RemoveAllListeners();
        DbsButton.onClick.RemoveAllListeners();
        DbsButton.onClick.RemoveAllListeners();

        AplButton.onClick.RemoveAllListeners();
        DplButton.onClick.RemoveAllListeners();
        AdlButton.onClick.RemoveAllListeners();
        DdlButton.onClick.RemoveAllListeners();
        PauseButton.onClick.RemoveAllListeners();

#endregion
    }

    void Start()
    {
        // In order to test and process messages we need these options.
        // However you can remove them if They do not suit your design.
        Application.runInBackground = true;
        Input.multiTouchEnabled     = false;

        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
        }

        StartNLog();

        gameObject.AddComponent<OnGuiDebugger>();

#region DEBUG

        ResetButton.onClick.AddListener(ResetButtonHandler);
        IbsButton.onClick.AddListener(IbsButtonHandler);
        DbsButton.onClick.AddListener(DbsButtonHandler);

        AplButton.onClick.AddListener(AplButtonHandler);
        DplButton.onClick.AddListener(DplButtonHandler);
        AdlButton.onClick.AddListener(AdButtonHandler);
        DdlButton.onClick.AddListener(DdButtonHandler);
        PauseButton.onClick.AddListener(PauseButtonHandler);

#endregion

        _ecsManager.Start();

        new Thread(() =>
        {
            _client = GameClient.Instance;
            _client.SetCallback(a => _messageConsumer.AddMessage(a));
            yojimbo.printf(yojimbo.LOG_LEVEL_INFO, "Connecting client");
            _client.Start();
        }).Start();
    }

    /// <summary>
    /// I have added NLog library to see your logs when you are testing your
    /// Project. You just need to check enable log, and enter your server address and port,
    /// In NLog prefab which is located in scene.
    /// </summary>
    private void StartNLog()
    {
        if (EnableNLog)
        {
            LogAdapter.Connect();
        }
    }

    /// <summary>
    /// Updates server messages.
    /// </summary>
    private void Update()
    {
        _messageConsumer.Update(_ecsManager);
    }

#region DEBUG_BUTTON_HANDLERS

    private void PauseButtonHandler()
    {
        var pauseCommand = _client.CreateMessage<PauseCommand>(GameMessageType.Pause);
        _client.SendMessage(GameChannelType.Reliable, pauseCommand);
    }

    private void AplButtonHandler()
    {
        _testPackageLoss += 1f;
        _testLatency     =  Mathf.Clamp(_testPackageLoss, 0, 100);
        _client.SetPackageLost(_testPackageLoss);
    }

    private void DplButtonHandler()
    {
        _testPackageLoss -= 1f;
        _testLatency     =  Mathf.Clamp(_testPackageLoss, 0, 100);
        _client.SetPackageLost(_testPackageLoss);
    }

    private void AdButtonHandler()
    {
        _testLatency += 10;
        _testLatency =  Mathf.Clamp(_testLatency, 0, 5000);
        _client.SetLatency(_testLatency);
    }

    private void DdButtonHandler()
    {
        _testLatency -= 10;
        _testLatency =  Mathf.Clamp(_testLatency, 0, 5000);
        _client.SetLatency(_testLatency);
    }

    private void DbsButtonHandler()
    {
        _client.SendMessage(GameChannelType.Reliable, new ChangeBallSpeed_Test {delta = -Constants.BallPace * 0.2f});
    }

    private void IbsButtonHandler()
    {
        _client.SendMessage(GameChannelType.Reliable, new ChangeBallSpeed_Test {delta = Constants.BallPace * 0.2f});
    }

    private void ResetButtonHandler()
    {
        _client.SendMessage(GameChannelType.Reliable, new ResetCommand());
    }

#endregion

    void OnApplicationQuit()
    {
        // Remove connection so that there is no memory leak or port invasion. 
        _client.Quit();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}