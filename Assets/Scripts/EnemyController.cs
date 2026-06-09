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
    private NormalAttack normalAttack;
    private Vector3 hudOffset;
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

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        enemyRigidbody.isKinematic = true;
        navMeshAgent.updateRotation = false;

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
        ResetHealth();
    }

    private void Update()
    {
        UpdateHud();

        if (isDead || IsRespawning)
        {
            return;
        }

        switch (state)
        {
            case EnemyState.Chase:
                UpdateDestination();
                FacePlayer();
                TryStartAttack();
                break;
            case EnemyState.Windup:
                HoldPosition();
                FacePlayer();
                if (Time.time >= stateEndTime)
                {
                    ShootIfPlayerIsInRange();
                    nextFireTime = Time.time + normalAttack.FireRate;
                    state = EnemyState.Recover;
                    stateEndTime = Time.time + attackRecoveryDuration;
                }
                break;
            case EnemyState.Recover:
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
        if (IsPlayerInAttackRange(out _))
        {
            HoldPosition();
            return;
        }

        if (Time.time < nextPathRefreshTime || !navMeshAgent.enabled)
        {
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(playerTarget.position);
        nextPathRefreshTime = Time.time + PathRefreshInterval;
    }

    private void TryStartAttack()
    {
        if (Time.time < nextFireTime || !IsPlayerInAttackRange(out Vector3 directionToPlayer))
        {
            return;
        }

        state = EnemyState.Windup;
        stateEndTime = Time.time + attackWindupDuration;
        HoldPosition();
        FaceDirection(directionToPlayer);
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
