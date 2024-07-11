using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public static GameManager Instance {  get; private set; }


	public event EventHandler OnStartChanged;
	public event EventHandler OnGamePaused;
	public event EventHandler OnGameUnpaused;


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

	private State state;
    private float countdownToStartTimer = 1f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 300f;
	private bool isGamePaused = false;



    private void Awake()
    {
		Instance = this;

		state = State.WaitngToStart;    
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAciton += GameInput_OnPauseAciton;
        GameInput.Instance.OnInteractAction += GameManager_OnInteractAction;

        //触发游戏自动启动
        state = State.CountdownToStart;
        OnStartChanged?.Invoke(this, EventArgs.Empty);

    }

    private void GameManager_OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.WaitngToStart)
        {
			state = State.CountdownToStart;
			OnStartChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAciton(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch (state)
		{
			case State.WaitngToStart:
				break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
					gamePlayingTimer = gamePlayingTimerMax;
                    OnStartChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStartChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
		
    }


	public bool IsGamePlaying()
	{
		return state == State.GamePlaying;
	}

	public bool IsCountdownToStartActive() 
	{
		return state == State.CountdownToStart;
	}

	public float GetCountdownToStartTimer()
	{
		return countdownToStartTimer;
	}

	public bool IsGameOver()
	{
		return state == State.GameOver;
	}

	public float GetGamePlayingTimerNormalized()
	{
		return 1 - (gamePlayingTimer / gamePlayingTimerMax);
	}

	public void TogglePauseGame()
	{
		isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;

			OnGamePaused?.Invoke(this,EventArgs.Empty);
        }
        else
        {
			Time.timeScale = 1f;

			OnGameUnpaused?.Invoke(this,EventArgs.Empty);
        }

    }
}
