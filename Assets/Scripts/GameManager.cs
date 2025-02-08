using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform player;

    [HideInInspector]
    public bool isRunnning;

    public GameState currentState;
    public static Action<GameState> OnStateChange;
    public static bool isRestart = false;

    private float numberOfDieEnemies;
    private float numberOfEnemies;

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
        numberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Count();
    }

    private void Start()
    {
        GameObject[] cannons = GameObject.FindGameObjectsWithTag("Cannon");
        foreach (var cannon in cannons)
        {
            cannon.GetComponent<Cannon>().Init();
        }
        UIManager.Instance.Init();
        if (!isRestart)
        {
            UpdateState(GameState.Start);
            isRunnning = false;
        }
        else
        {
            UpdateState(GameState.Running);
        }
    }

    public void IsRestart(bool value)
    {
        isRestart = value;
    }

    public void NotifyOnHit(Collider2D objectGetHit)
    {
        var hitable = objectGetHit.GetComponent<IHitable>();
        if (hitable == null)
        {
            return;
        }
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
            if(numberOfDieEnemies == numberOfEnemies)
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
