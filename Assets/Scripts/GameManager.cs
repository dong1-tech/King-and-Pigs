using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform player;

    public bool isRunnning;

    public GameState currentState;
    public static Action<GameState> OnStateChange;

    private float numberOfDieEnemies;

    public void UpdateState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.Start:
                break;
            case GameState.Running:
                GameRunning();
                break;
            case GameState.Pause:
                GamePause();
                break;
            case GameState.Lose:
                GameLose();
                break;
            case GameState.Victory:
                GameVictory();
                break;
        }
        OnStateChange?.Invoke(currentState);
    }

    private void GameVictory()
    {
        isRunnning = false;
    }

    private void GameLose()
    {
        isRunnning = false;
    }

    private void GamePause()
    {
        isRunnning = false;
        Time.timeScale = 0;
    }

    private void GameRunning()
    {
        isRunnning = true;
        Time.timeScale = 1;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UIManager.Instance.Init();
        UpdateState(GameState.Start);
        isRunnning = false;
    }

    public void NotifyOnHit(Collider2D objectGetHit)
    {
        IHitable hitable = objectGetHit.GetComponent<IHitable>();
        float remainHealth = (float)hitable?.OnHit();
        UIManager.Instance.HealthBarUpdate(objectGetHit, remainHealth);
    }
    public void NotifyOnDead(string name)
    {
        if(name == "Player")
        {
            UpdateState(GameState.Lose);
        } else
        {
            numberOfDieEnemies += 1;
            if(numberOfDieEnemies == 2)
            {
                Invoke("Victory", 2f);
            }
        }
    }
    void Victory()
    {
        UpdateState(GameState.Victory);
    }
}
public enum GameState
{
    Start, Running, Pause, Lose, Victory
}
