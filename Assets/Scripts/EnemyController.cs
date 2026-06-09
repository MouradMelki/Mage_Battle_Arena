using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private enum EnemyState
    {
        Chase,
        Windup,
        Recover,
        Dead,
        Respawning
    }

    private const string FriendlyFireTag = "FriendlyFire";
    private const string EnemyFireTag = "EnemyFire";
    private const float PathRefreshInterval = 0.15f;
    private const float MinAimSqrMagnitude = 0.0001f;
    private const float TelegraphPulseSpeed = 8f;

    [Header("Combat")]
    [SerializeField, FormerlySerializedAs("m_projectile")]
    private GameObject projectilePrefab;
    [SerializeField, FormerlySerializedAs("m_firePoint")]
    private Transform firePoint;
    [SerializeField]
    private float fireRate = 1f;
    [SerializeField]
    private float firePointDistance = 1.15f;
    [SerializeField]
    private float orbitRadius = 1f;
    [SerializeField]
    private float idealAttackRange = 7.5f;
    [SerializeField]
    private float tooCloseRange = 4f;
    [SerializeField]
    private float projectileSpeed = 6f;
    [SerializeField]
    private float projectileDamage = 20f;
    [SerializeField]
    private float projectileRange = 8f;
    [SerializeField]
    private float attackWindupDuration = 0.35f;
    [SerializeField]
    private float attackRecoveryDuration = 0.35f;
    [SerializeField]
    private float rotationSpeed = 540f;
    [SerializeField]
    private Color attackTelegraphColor = new Color(1f, 0.35f, 0f, 1f);

    [Header("Health")]
    [SerializeField]
    private float startingHealth = 100f;
    [SerializeField]
    private Slider healthSlider;

    [Header("Scene References")]
    [SerializeField, FormerlySerializedAs("m_Canvas_HUD")]
    private Transform hudCanvas;
    [SerializeField, FormerlySerializedAs("RespawnPos")]
    private Transform respawnPoint;
    [SerializeField]
    private Transform playerTarget;

    private Rigidbody enemyRigidbody;
    private NavMeshAgent navMeshAgent;
    private Renderer enemyRenderer;
    private NormalAttack normalAttack;
    private Vector3 hudOffset;
    private Color startingColor;
    private float currentHealth;
    private float nextFireTime;
    private float nextPathRefreshTime;
    private float stateEndTime;
    private bool isDead;
    private EnemyState state = EnemyState.Chase;

    public bool IsDead => isDead;
    public bool IsRespawning { get; private set; }
    public Transform RespawnPoint => respawnPoint;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponentInChildren<Renderer>();

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        enemyRigidbody.isKinematic = true;
        navMeshAgent.updateRotation = false;
        navMeshAgent.stoppingDistance = 0f;

        normalAttack = new NormalAttack
        {
            FireRate = fireRate,
            FirePointDistance = firePointDistance,
            SpeedOfAttack = projectileSpeed,
            Damage = projectileDamage,
            Range = projectileRange,
            TagProjectile = EnemyFireTag
        };

        hudOffset = hudCanvas.position - transform.position;
        if (enemyRenderer)
        {
            startingColor = enemyRenderer.material.color;
        }

        ResetHealth();
    }

    private void Update()
    {
        UpdateHud();

        if (isDead || IsRespawning)
        {
            return;
        }

        UpdateTelegraph();

        switch (state)
        {
            case EnemyState.Chase:
                UpdateDestination();
                FacePlayer();
                TryStartAttack();
                break;
            case EnemyState.Windup:
                if (TryMoveAwayIfTooClose())
                {
                    break;
                }

                HoldPosition();
                FacePlayer();
                if (Time.time >= stateEndTime)
                {
                    ShootIfPlayerIsInRange();
                    nextFireTime = Time.time + normalAttack.FireRate;
                    state = EnemyState.Recover;
                    ResetTelegraph();
                    stateEndTime = Time.time + attackRecoveryDuration;
                }
                break;
            case EnemyState.Recover:
                if (TryMoveAwayIfTooClose())
                {
                    break;
                }

                HoldPosition();
                FacePlayer();
                if (Time.time >= stateEndTime)
                {
                    state = EnemyState.Chase;
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(FriendlyFireTag))
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out ProjectileBehaviour projectile))
        {
            TakeDamage(projectile.Damage);
        }
        else
        {
            Debug.LogError("Friendly projectile is missing ProjectileBehaviour.", collision.gameObject);
        }
    }

    public void BeginRespawn()
    {
        IsRespawning = true;
        state = EnemyState.Respawning;
        ResetTelegraph();
        if (navMeshAgent)
        {
            navMeshAgent.enabled = false;
        }
    }

    public void CompleteRespawn()
    {
        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        ResetHealth();
        IsRespawning = false;
        state = EnemyState.Chase;
        ResetTelegraph();

        if (navMeshAgent)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.Warp(transform.position);
        }
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        valid &= LogMissing(enemyRigidbody, "Rigidbody");
        valid &= LogMissing(navMeshAgent, "NavMeshAgent");
        valid &= LogMissing(projectilePrefab, nameof(projectilePrefab));
        valid &= LogMissing(firePoint, nameof(firePoint));
        valid &= LogMissing(healthSlider, nameof(healthSlider));
        valid &= LogMissing(hudCanvas, nameof(hudCanvas));
        valid &= LogMissing(respawnPoint, nameof(respawnPoint));
        valid &= LogMissing(playerTarget, nameof(playerTarget));

        return valid;
    }

    private bool LogMissing(Object reference, string referenceName)
    {
        if (reference)
        {
            return true;
        }

        Debug.LogError($"{nameof(EnemyController)} on {name} is missing required reference: {referenceName}.", this);
        return false;
    }

    private void UpdateHud()
    {
        hudCanvas.rotation = Quaternion.LookRotation(Vector3.down);
        hudCanvas.position = transform.position + hudOffset;
    }

    private void UpdateDestination()
    {
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        directionToPlayer.y = 0f;
        float distanceSqr = directionToPlayer.sqrMagnitude;

        if (distanceSqr <= tooCloseRange * tooCloseRange)
        {
            MoveAwayFromPlayer(directionToPlayer, true);
            return;
        }

        if (distanceSqr <= idealAttackRange * idealAttackRange)
        {
            HoldPosition();
            return;
        }

        MoveTowardPlayer();
    }

    private void MoveTowardPlayer()
    {
        UpdateAgentDestination(playerTarget.position);
    }

    private void MoveAwayFromPlayer(Vector3 directionToPlayer, bool force = false)
    {
        Vector3 retreatDirection = -directionToPlayer;
        if (retreatDirection.sqrMagnitude <= MinAimSqrMagnitude)
        {
            retreatDirection = -transform.forward;
        }

        Vector3 destination = playerTarget.position + retreatDirection.normalized * idealAttackRange;
        UpdateAgentDestination(destination, force);
    }

    private void UpdateAgentDestination(Vector3 destination, bool force = false)
    {
        if ((!force && Time.time < nextPathRefreshTime) || !navMeshAgent.enabled)
        {
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
        nextPathRefreshTime = Time.time + PathRefreshInterval;
    }

    private void TryStartAttack()
    {
        if (Time.time < nextFireTime || !IsPlayerInIdealRange(out Vector3 directionToPlayer))
        {
            return;
        }

        state = EnemyState.Windup;
        stateEndTime = Time.time + attackWindupDuration;
        HoldPosition();
        FaceDirection(directionToPlayer);
    }

    private bool TryMoveAwayIfTooClose()
    {
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        directionToPlayer.y = 0f;
        if (directionToPlayer.sqrMagnitude > tooCloseRange * tooCloseRange)
        {
            return false;
        }

        state = EnemyState.Chase;
        ResetTelegraph();
        MoveAwayFromPlayer(directionToPlayer, true);
        FaceDirection(directionToPlayer);
        return true;
    }

    private bool IsPlayerInIdealRange(out Vector3 directionToPlayer)
    {
        directionToPlayer = playerTarget.position - transform.position;
        directionToPlayer.y = 0f;

        float distanceSqr = directionToPlayer.sqrMagnitude;
        return distanceSqr > tooCloseRange * tooCloseRange && distanceSqr <= idealAttackRange * idealAttackRange;
    }

    private void UpdateTelegraph()
    {
        if (!enemyRenderer || state != EnemyState.Windup)
        {
            ResetTelegraph();
            return;
        }

        float pulse = Mathf.PingPong(Time.time * TelegraphPulseSpeed, 1f);
        enemyRenderer.material.color = Color.Lerp(startingColor, attackTelegraphColor, pulse);
    }

    private void ResetTelegraph()
    {
        if (enemyRenderer)
        {
            enemyRenderer.material.color = startingColor;
        }
    }

    private bool IsPlayerInAttackRange(out Vector3 directionToPlayer)
    {
        directionToPlayer = playerTarget.position - transform.position;
        float range = normalAttack.Range + orbitRadius;
        return directionToPlayer.sqrMagnitude <= range * range;
    }

    private void ShootIfPlayerIsInRange()
    {
        if (!IsPlayerInAttackRange(out Vector3 directionToPlayer))
        {
            return;
        }

        Shoot(directionToPlayer);
    }

    private void HoldPosition()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.nextPosition = transform.position;
            navMeshAgent.ResetPath();
        }
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        FaceDirection(directionToPlayer);
    }

    private void FaceDirection(Vector3 direction)
    {
        direction.y = 0f;
        if (direction.sqrMagnitude <= MinAimSqrMagnitude)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot(Vector3 direction)
    {
        if (direction.sqrMagnitude <= MinAimSqrMagnitude)
        {
            return;
        }

        direction.Normalize();
        normalAttack.Direction = direction;
        firePoint.position = transform.position + direction * normalAttack.FirePointDistance;

        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectileObject.tag = normalAttack.TagProjectile;

        if (projectileObject.TryGetComponent(out Rigidbody projectileRigidbody))
        {
            projectileRigidbody.linearVelocity = direction * normalAttack.SpeedOfAttack;
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody.", projectileObject);
        }

        if (projectileObject.TryGetComponent(out ProjectileBehaviour projectile))
        {
            projectile.Configure(normalAttack);
        }
        else
        {
            Debug.LogError("Projectile prefab is missing ProjectileBehaviour.", projectileObject);
        }

        FaceDirection(direction);
    }

    private void TakeDamage(float amount)
    {
        if (isDead || IsRespawning)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthSlider.value = currentHealth;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        state = EnemyState.Dead;
        ResetTelegraph();
        if (navMeshAgent)
        {
            navMeshAgent.enabled = false;
        }

        if (GameController.Instance)
        {
            GameController.Instance.RespawnEnemy(this);
        }
        else
        {
            Debug.LogError($"{nameof(EnemyController)} cannot respawn because no {nameof(GameController)} exists.", this);
        }
    }

    private void ResetHealth()
    {
        currentHealth = startingHealth;
        healthSlider.maxValue = startingHealth;
        healthSlider.value = startingHealth;
        isDead = false;
    }
}
