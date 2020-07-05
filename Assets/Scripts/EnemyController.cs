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

    private Vector3 m_offset_hud;
    private float nextFire;
    private bool shoot = false;
    protected readonly GameObject projectil;
    private readonly string enemyFire = "FriendlyFire";

    private void Awake()
    {
        m_offset_hud = m_Canvas_HUD.position - transform.position;

        EnemyBot = new EnemyBot()
        {
            FireRate = 1f,
            Projectile = m_projectile,
            EnemyTransform = transform,
            FirePoint = m_firePoint,
            FirePointDistance = 1.15f,
            OrbitRadius = 1f,
            Player = GameObject.FindGameObjectWithTag("Player"),
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
                TagProjectile = "EnemyFire"
            }
        };
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

        if (EnemyBot.IsDead)
        {
            GameController.gameController.RespawnPlayer(this);
        }
    }

    private void FixedUpdate()
    {
        EnemyBot.MovementControl();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(enemyFire))
        {
            EnemyBot.TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>().NormalAttack.Damage);
        }
    }
}
