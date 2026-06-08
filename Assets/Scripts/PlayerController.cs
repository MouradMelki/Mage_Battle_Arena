using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const string EnemyFireTag = "EnemyFire";
    private const string FriendlyFireTag = "FriendlyFire";
    private const float MinMoveSqrMagnitude = 0.0001f;

    [Header("Movement")]
    [SerializeField, FormerlySerializedAs("m_speed")]
    private float moveSpeed = 5f;

    [Header("Combat")]
    [SerializeField, FormerlySerializedAs("m_firePoint")]
    private Transform firePoint;
    [SerializeField, FormerlySerializedAs("m_projectile")]
    private GameObject projectilePrefab;
    [SerializeField]
    private float projectileSpeed = 6f;
    [SerializeField]
    private float projectileDamage = 200f;
    [SerializeField]
    private float projectileRange = 8f;
    [SerializeField]
    private float fireRate = 1f;
    [SerializeField]
    private float firePointDistance = 1.15f;
    [SerializeField]
    private float noAimZoneRadius = 0.2f;
    [SerializeField]
    private LayerMask enemyLayerMask;

    [Header("Aim UI")]
    [SerializeField, FormerlySerializedAs("m_Canvas_Attack_HUD")]
    private Rigidbody attackHudBody;
    [SerializeField, FormerlySerializedAs("m_Canvas_Attack_HUD_Aim")]
    private RectTransform attackHudAim;
    [SerializeField]
    private float aimScaleSpeed = 1f;
    [SerializeField]
    private float aimMaxScale = 10f;
    [SerializeField]
    private float orbitRadius = 1f;

    [Header("Health")]
    [SerializeField]
    private float startingHealth = 100f;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Image damageImage;
    [SerializeField]
    private float flashSpeed = 5f;
    [SerializeField]
    private Color flashColour = new Color(1f, 0f, 0f, 0.35f);

    [Header("Scene References")]
    [SerializeField]
    private FloatingJoystick leftJoystick;
    [SerializeField]
    private FloatingJoystick rightJoystick;
    [SerializeField, FormerlySerializedAs("m_Canvas_HUD")]
    private Transform hudCanvas;
    [SerializeField, FormerlySerializedAs("RespawnPos")]
    private Transform respawnPoint;

    private readonly Collider[] autoAimHits = new Collider[16];
    private Rigidbody playerRigidbody;
    private NormalAttack normalAttack;
    private Vector3 hudOffset;
    private Vector3 attackHudOffset;
    private Vector3 aimStartingScale;
    private Vector3 queuedShotDirection;
    private bool leftJoystickActive;
    private bool rightJoystickActive;
    private bool shootRequested;
    private bool damagedThisFrame;
    private float currentHealth;
    private float nextFireTime;
    private bool isDead;

    public bool IsDead => isDead;
    public bool IsRespawning { get; private set; }
    public float NoAimZoneRadius => noAimZoneRadius;
    public Transform RespawnPoint => respawnPoint;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();

        if (enemyLayerMask == 0)
        {
            enemyLayerMask = LayerMask.GetMask("Enemy");
        }

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        normalAttack = new NormalAttack
        {
            SpeedOfAttack = projectileSpeed,
            Damage = projectileDamage,
            Range = projectileRange,
            TagProjectile = FriendlyFireTag
        };

        hudOffset = hudCanvas.position - transform.position;
        attackHudOffset = attackHudBody.position - transform.position;
        aimStartingScale = attackHudAim.localScale;

        leftJoystick.PlayerController = this;
        rightJoystick.PlayerController = this;

        ResetHealth();
        ClearDamageFlash();
    }

    private void Update()
    {
        UpdateAttachedHud();
        UpdateAimHud();
        UpdateDamageFlash();
    }

    private void FixedUpdate()
    {
        if (!isDead && leftJoystickActive)
        {
            MoveFromJoystick();
        }

        if (shootRequested)
        {
            if (!isDead)
            {
                Shoot(queuedShotDirection);
            }

            shootRequested = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(EnemyFireTag))
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out ProjectileBehaviour projectile))
        {
            TakeDamage(projectile.Damage);
        }
        else
        {
            Debug.LogError("Enemy projectile is missing ProjectileBehaviour.", collision.gameObject);
        }
    }

    public void SetLeftJoystickActive(bool active)
    {
        leftJoystickActive = active;
    }

    public void SetRightJoystickActive(bool active)
    {
        rightJoystickActive = active;
    }

    public void QueueShot(Vector3 direction)
    {
        if (isDead || Time.time < nextFireTime)
        {
            return;
        }

        queuedShotDirection = direction;
        shootRequested = true;
        nextFireTime = Time.time + fireRate;
    }

    public void BeginRespawn()
    {
        IsRespawning = true;
    }

    public void CompleteRespawn()
    {
        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.identity;
        ResetHealth();
        rightJoystickActive = false;
        leftJoystickActive = false;
        shootRequested = false;
        IsRespawning = false;
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        valid &= LogMissing(playerRigidbody, "Rigidbody");
        valid &= LogMissing(leftJoystick, nameof(leftJoystick));
        valid &= LogMissing(rightJoystick, nameof(rightJoystick));
        valid &= LogMissing(healthSlider, nameof(healthSlider));
        valid &= LogMissing(damageImage, nameof(damageImage));
        valid &= LogMissing(firePoint, nameof(firePoint));
        valid &= LogMissing(projectilePrefab, nameof(projectilePrefab));
        valid &= LogMissing(hudCanvas, nameof(hudCanvas));
        valid &= LogMissing(attackHudBody, nameof(attackHudBody));
        valid &= LogMissing(attackHudAim, nameof(attackHudAim));
        valid &= LogMissing(respawnPoint, nameof(respawnPoint));

        return valid;
    }

    private bool LogMissing(Object reference, string referenceName)
    {
        if (reference)
        {
            return true;
        }

        Debug.LogError($"{nameof(PlayerController)} on {name} is missing required reference: {referenceName}.", this);
        return false;
    }

    private void MoveFromJoystick()
    {
        Vector3 direction = new Vector3(leftJoystick.Horizontal, 0f, leftJoystick.Vertical);
        if (direction.sqrMagnitude <= MinMoveSqrMagnitude)
        {
            return;
        }

        Vector3 normalizedDirection = direction.normalized;
        Vector3 movement = normalizedDirection * moveSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + movement);
        playerRigidbody.MoveRotation(Quaternion.LookRotation(normalizedDirection));
    }

    private void Shoot(Vector3 requestedDirection)
    {
        Vector3 shotDirection = GetShotDirection(requestedDirection);
        if (shotDirection.sqrMagnitude <= MinMoveSqrMagnitude)
        {
            return;
        }

        shotDirection.Normalize();
        normalAttack.Direction = shotDirection;

        firePoint.position = transform.position + shotDirection * firePointDistance;
        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectileObject.tag = normalAttack.TagProjectile;

        if (projectileObject.TryGetComponent(out Rigidbody projectileRigidbody))
        {
            projectileRigidbody.linearVelocity = shotDirection * normalAttack.SpeedOfAttack;
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

        playerRigidbody.MoveRotation(Quaternion.LookRotation(shotDirection));
    }

    private Vector3 GetShotDirection(Vector3 requestedDirection)
    {
        if (requestedDirection.sqrMagnitude >= noAimZoneRadius * noAimZoneRadius)
        {
            return requestedDirection;
        }

        float aimAngle = Mathf.PI * playerRigidbody.rotation.eulerAngles.y / 180f;
        Vector3 forwardDirection = new Vector3(Mathf.Sin(aimAngle), 0f, Mathf.Cos(aimAngle));
        return AimAutomaticallyAtEnemy(forwardDirection);
    }

    private Vector3 AimAutomaticallyAtEnemy(Vector3 fallbackDirection)
    {
        float range = normalAttack.Range + orbitRadius;
        float closestSqrDistance = range * range;
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, autoAimHits, enemyLayerMask);
        Vector3 bestDirection = fallbackDirection;

        for (int i = 0; i < hitCount; i++)
        {
            Vector3 enemyDirection = autoAimHits[i].transform.position - transform.position;
            float sqrDistance = enemyDirection.sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                bestDirection = enemyDirection;
            }
        }

        return bestDirection;
    }

    private void TakeDamage(float amount)
    {
        if (isDead || IsRespawning)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthSlider.value = currentHealth;
        damagedThisFrame = currentHealth > 0f;

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
        if (GameController.Instance)
        {
            GameController.Instance.RespawnPlayer(this);
        }
        else
        {
            Debug.LogError($"{nameof(PlayerController)} cannot respawn because no {nameof(GameController)} exists.", this);
        }
    }

    private void ResetHealth()
    {
        currentHealth = startingHealth;
        healthSlider.maxValue = startingHealth;
        healthSlider.value = startingHealth;
        isDead = false;
    }

    private void UpdateAttachedHud()
    {
        attackHudBody.position = transform.position + attackHudOffset;
        hudCanvas.position = transform.position + hudOffset;
        hudCanvas.rotation = Quaternion.LookRotation(new Vector3(0f, -90f, 0f));
    }

    private void UpdateAimHud()
    {
        if (rightJoystickActive)
        {
            float zAimRotation = Mathf.Atan2(rightJoystick.Horizontal, rightJoystick.Vertical) * Mathf.Rad2Deg;
            float zAimPosition = attackHudAim.localScale.y / 2f + orbitRadius;
            Vector3 aimDirection = new Vector3(rightJoystick.Horizontal, rightJoystick.Vertical, 0f);

            attackHudAim.localScale = new Vector3(
                attackHudAim.localScale.x,
                Mathf.Lerp(attackHudAim.localScale.y, aimMaxScale, aimScaleSpeed * Time.deltaTime),
                attackHudAim.localScale.z);

            if (aimDirection.sqrMagnitude > MinMoveSqrMagnitude)
            {
                aimDirection.Normalize();
                attackHudAim.localPosition = aimDirection * zAimPosition;
            }

            attackHudBody.MoveRotation(Quaternion.Euler(90f, zAimRotation, 0f));
        }
        else if (attackHudAim.localScale != aimStartingScale)
        {
            attackHudAim.localScale = aimStartingScale;
            attackHudAim.position = attackHudBody.position;
        }
    }

    private void UpdateDamageFlash()
    {
        if (damagedThisFrame)
        {
            damageImage.color = flashColour;
            damagedThisFrame = false;
            return;
        }

        damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
    }

    private void ClearDamageFlash()
    {
        damageImage.color = Color.clear;
    }
}
