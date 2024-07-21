using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

	public static GameManager Instance {  get; private set; }


	public event EventHandler OnStartChanged;
	public event EventHandler OnLocalGamePaused;
	public event EventHandler OnLocalGameUnpaused;
	public event EventHandler OnMultiplayerGamePaused;
	public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;


	private enum State
	{
		/// <summary>
		/// 等待启动
		/// </summary>
		WaitngToStart,
		/// <summary>
		/// 倒计时开始
		/// </summary>
		CountdownToStart,
		/// <summary>
		/// 游戏开始
		/// </summary>
		GamePlaying,
		/// <summary>
		/// 游戏结束
		/// </summary>
		GameOver,
	}

	private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitngToStart);
	private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 120f;
	private bool isLocalGamePaused = false;
	private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
	private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;




    private void Awake()
    {
		Instance = this;


        playerReadyDictionary = new Dictionary<ulong, bool>();
		playerPausedDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAciton += GameInput_OnPauseAciton;
        GameInput.Instance.OnInteractAction += GameManager_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
		state.OnValueChanged += State_OnValueChanged;
		isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;

            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);

        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStartChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameManager_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitngToStart)
        {
			isLocalPlayerReady = true;

            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

		bool allClientsReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerReadyDictionary.ContainsKey(clientId) == false || playerReadyDictionary[clientId] == false)
            {
                //玩家还没准备好
				allClientsReady = false;
				break;
            }
        }

		if (allClientsReady)
		{
			state.Value = State.CountdownToStart;
		}
    }

    private void GameInput_OnPauseAciton(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer)
        {
			return;
        }

        switch (state.Value)
		{
			case State.WaitngToStart:
				break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
					gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
		
    }


	public bool IsGamePlaying()
	{
		return state.Value == State.GamePlaying;
	}

	public bool IsCountdownToStartActive() 
	{
		return state.Value == State.CountdownToStart;
	}

	public float GetCountdownToStartTimer()
	{
		return countdownToStartTimer.Value;
	}

	public bool IsGameOver()
	{
		return state.Value == State.GameOver;
	}

	public bool IsLocalPlayerReady()
	{
		return isLocalPlayerReady;
	}

	public float GetGamePlayingTimerNormalized()
	{
		return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
	}

	public void TogglePauseGame()
	{
		isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
			PauseGameServerRpc();

			OnLocalGamePaused?.Invoke(this,EventArgs.Empty);
        }
        else
        {
			UnPauseGameServerRpc();

			OnLocalGameUnpaused?.Invoke(this,EventArgs.Empty);
        }

    }

	[ServerRpc(RequireOwnership = false)]
	private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

		TestGamePausedState();
    }

	private void TestGamePausedState()
	{
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
				//这个玩家暂停
				isGamePaused.Value = true;
				return;
            }
        }
        //没有玩家暂停
        isGamePaused.Value = false;

    }

}
