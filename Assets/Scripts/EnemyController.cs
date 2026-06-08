using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Diagnostics.Contracts;

public class EnemyController : MonoBehaviour
{
    public GameObject m_projectile;
    public Transform m_firePoint;
    public Transform m_Canvas_HUD;
    public Slider healthSlider;
    public EnemyBot EnemyBot { get; set; }
    public Transform RespawnPos;
    public bool IsRespawning { get; set; }

    private Vector3 m_offset_hud;
    private float nextFire;
    private bool shoot = false;
    protected readonly GameObject projectil;
    private const string FriendlyFireTag = "FriendlyFire";
    private const string EnemyFireTag = "EnemyFire";
    private const string PlayerTag = "Player";

    private void Awake()
    {
        if (!m_Canvas_HUD || !m_projectile || !m_firePoint || !healthSlider || !RespawnPos)
        {
            Debug.LogError("EnemyController is missing one or more required scene references.", this);
            enabled = false;
            return;
        }

        m_offset_hud = m_Canvas_HUD.position - transform.position;

        EnemyBot = new EnemyBot()
        {
            FireRate = 1f,
            Projectile = m_projectile,
            EnemyTransform = transform,
            FirePoint = m_firePoint,
            FirePointDistance = 1.15f,
            OrbitRadius = 1f,
            Player = GameObject.FindGameObjectWithTag(PlayerTag),
            Nav = GetComponent<NavMeshAgent>(),
            HealthSlider = healthSlider,
            CurrentHealth = 100f,
            StartingHealth = 100f,
            EnemyRigidbody = GetComponent<Rigidbody>(),
            IsDead = false,
            NormalAttack = new NormalAttack
            {
                SpeedOfAttack = 6f,
                Damage = 20f,
                Range = 8f,
                TagProjectile = EnemyFireTag
            }
        };

        if (!EnemyBot.Nav)
        {
            Debug.LogError("EnemyController requires a NavMeshAgent component.", this);
            enabled = false;
        }
    }

    void Update()
    {
        // Fire Rate Control
        if (Time.time > nextFire && shoot == false)
        {
            nextFire = Time.time + EnemyBot.FireRate;
            shoot = true;
        }

        if (shoot)
        {
            EnemyBot.Shoot();
            shoot = false;
        }

        m_Canvas_HUD.rotation = Quaternion.LookRotation(Vector3.down);
        m_Canvas_HUD.position = transform.position + m_offset_hud;

        if (EnemyBot.IsDead && !IsRespawning)
        {
            if (GameController.gameController)
            {
                GameController.gameController.RespawnPlayer(this);
            }
            else
            {
                Debug.LogError("EnemyController cannot respawn because no GameController is available.", this);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsRespawning)
        {
            EnemyBot.MovementControl();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(FriendlyFireTag))
        {
            EnemyBot.TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>().NormalAttack.Damage);
        }
    }
}
