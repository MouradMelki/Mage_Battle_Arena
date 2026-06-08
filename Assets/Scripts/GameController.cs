using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public GameObject TimeUpPanel;

    public float Possetion { get; set; }

    private const string TimeUpPanelName = "TimeUpPannel";
    private const float ReSpawnTime = 5f;
    private const float MatchMinutes = 1f;
    private List<Observer> observers = new List<Observer>();

    private void Start()
    {
        Possetion = 0f;

        if (!TimeUpPanel)
        {
            TimeUpPanel = GameObject.Find(TimeUpPanelName);
        }

        if (TimeUpPanel)
        {
            TimeUpPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameController could not find TimeUpPannel. Match-end UI must be wired in the Unity Editor.", this);
        }

        new TimerUIObserver(this, MatchMinutes);
    }

    void Awake()
    {
        Time.timeScale = 1f;

        if (gameController == null)
        {
            gameController = this;
        } else if (gameController != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        NotifyAllObservers();
    }

    public void RespawnPlayer(PlayerController playerController)
    {
        if (!playerController || playerController.IsRespawning)
        {
            return;
        }

        playerController.IsRespawning = true;
        StartCoroutine(RespawnPlayerCoroutine(playerController));
    }

    public IEnumerator RespawnPlayerCoroutine(PlayerController playerController)
    {
        playerController.gameObject.SetActive(false);
        yield return new WaitForSeconds(ReSpawnTime);
        playerController.transform.position = playerController.RespawnPos.position;
        playerController.Player.CurrentHealth = playerController.Player.StartingHealth;
        playerController.Player.HealthSlider.value = playerController.Player.StartingHealth;
        playerController.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerController.Player.IsDead = false;
        playerController.IsRespawning = false;
        playerController.gameObject.SetActive(true);
    }

    public void RespawnPlayer(EnemyController enemyController)
    {
        if (!enemyController || enemyController.IsRespawning)
        {
            return;
        }

        enemyController.IsRespawning = true;
        StartCoroutine(RespawnPlayerCoroutine(enemyController));
    }

    public IEnumerator RespawnPlayerCoroutine(EnemyController enemyController)
    {
        enemyController.gameObject.SetActive(false);
        yield return new WaitForSeconds(ReSpawnTime);
        enemyController.transform.position = enemyController.RespawnPos.position;
        enemyController.EnemyBot.CurrentHealth = enemyController.EnemyBot.StartingHealth;
        enemyController.EnemyBot.HealthSlider.value = enemyController.EnemyBot.StartingHealth;
        enemyController.transform.rotation = Quaternion.Euler(0f,180f,0f);
        enemyController.EnemyBot.IsDead = false;
        if (enemyController.EnemyBot.Nav)
        {
            enemyController.EnemyBot.Nav.enabled = true;
        }
        enemyController.IsRespawning = false;
        enemyController.gameObject.SetActive(true);
    }

    public void ShowTimeUpPanel()
    {
        if (TimeUpPanel)
        {
            TimeUpPanel.SetActive(true);
        }
    }

    public bool GameWon()
    {
        if (Possetion > 50f)
        {
            return true;
        }

        return false;
    }

    public void Attach(Observer observer)
    {
        observers.Add(observer);
    }

    public void NotifyAllObservers()
    {
        observers.ForEach(delegate (Observer observer) {
            observer.UpdateObserver();
        });
    }
}
