using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Match")]
    [SerializeField]
    private float matchDurationSeconds = 60f;
    [SerializeField]
    private Text timerText;
    [SerializeField, FormerlySerializedAs("TimeUpPanel")]
    private GameObject timeUpPanel;

    [Header("Respawn")]
    [SerializeField]
    private float respawnDelaySeconds = 5f;

    private WaitForSeconds respawnDelay;
    private float matchStartTime;
    private bool matchEnded;

    private void Awake()
    {
        Time.timeScale = 1f;
        respawnDelay = new WaitForSeconds(respawnDelaySeconds);

        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        matchStartTime = Time.time;
        timeUpPanel.SetActive(false);
        UpdateTimerText(0f);
    }

    private void Update()
    {
        if (matchEnded)
        {
            return;
        }

        float elapsedTime = Time.time - matchStartTime;
        UpdateTimerText(elapsedTime);

        if (elapsedTime >= matchDurationSeconds)
        {
            EndMatch();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f;
        }
    }

    public void RespawnPlayer(PlayerController playerController)
    {
        if (!playerController || playerController.IsRespawning)
        {
            return;
        }

        StartCoroutine(RespawnPlayerCoroutine(playerController));
    }

    public void RespawnEnemy(EnemyController enemyController)
    {
        if (!enemyController || enemyController.IsRespawning)
        {
            return;
        }

        StartCoroutine(RespawnEnemyCoroutine(enemyController));
    }

    private IEnumerator RespawnPlayerCoroutine(PlayerController playerController)
    {
        playerController.BeginRespawn();
        playerController.gameObject.SetActive(false);
        yield return respawnDelay;
        playerController.gameObject.SetActive(true);
        playerController.CompleteRespawn();
    }

    private IEnumerator RespawnEnemyCoroutine(EnemyController enemyController)
    {
        enemyController.BeginRespawn();
        enemyController.gameObject.SetActive(false);
        yield return respawnDelay;
        enemyController.gameObject.SetActive(true);
        enemyController.CompleteRespawn();
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        valid &= LogMissing(timerText, nameof(timerText));
        valid &= LogMissing(timeUpPanel, nameof(timeUpPanel));

        return valid;
    }

    private bool LogMissing(Object reference, string referenceName)
    {
        if (reference)
        {
            return true;
        }

        Debug.LogError($"{nameof(GameController)} is missing required reference: {referenceName}.", this);
        return false;
    }

    private void UpdateTimerText(float elapsedTime)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void EndMatch()
    {
        matchEnded = true;
        UpdateTimerText(matchDurationSeconds);
        timeUpPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
