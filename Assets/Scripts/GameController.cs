using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public float Possetion { get; set; }

    private readonly float ReSpawnTime = 5f;
    private List<Observer> observers = new List<Observer>();

    private void Start()
    {
        Possetion = 0f;
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
        new TimerUIObserver(this);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'
    }

    void Awake()
    {
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
        playerController.gameObject.SetActive(true);
    }

    public void RespawnPlayer(EnemyController enemyController)
    {
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
        enemyController.gameObject.SetActive(true);
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
