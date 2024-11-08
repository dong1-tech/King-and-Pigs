using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{ 
    public static UIManager Instance;

    [SerializeField] private GameObject startGameScene;
    [SerializeField] private GameObject pauseGameScene;
    [SerializeField] private GameObject loseGameScene;
    [SerializeField] private GameObject victoryGameScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GameManager.OnStateChange += OnEnableScene;
    }

    public void Init()
    {
        
    }

    private void OnDestroy()
    {
        GameManager.OnStateChange -= OnEnableScene;
    }

    private void OnEnableScene(GameState state)
    {
        startGameScene.SetActive(state == GameState.Start);
        pauseGameScene.SetActive(state == GameState.Pause);
        loseGameScene.SetActive(state == GameState.Lose);
        victoryGameScene.SetActive(state == GameState.Victory);
    }

    public void OnPlayGame()
    {
        GameManager.Instance.UpdateState(GameState.Running);
    }

    public void OnPauseGame()
    {
        GameManager.Instance.UpdateState(GameState.Pause);
    }

    public void OnRestartGame()
    {
        GameManager.Instance.IsRestart(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnHome()
    {
        GameManager.Instance.IsRestart(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HealthBarUpdate(Collider2D other, float healthRemain)
    {
        IHealthBar healthBar = other.GetComponent<IHealthBar>();
        healthBar?.UpdateHealth(healthRemain);
    }

}
