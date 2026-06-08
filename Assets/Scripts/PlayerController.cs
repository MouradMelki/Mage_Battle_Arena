using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float m_speed;
    public Slider healthSlider;
    public Player Player { get; set; }
    public bool shoot;
    public Transform m_firePoint;
    public Transform m_Canvas_HUD;
    public Transform RespawnPos;
    public Rigidbody m_Canvas_Attack_HUD;
    public GameObject m_projectile;
    public RectTransform m_Canvas_Attack_HUD_Aim;
    public bool IsRespawning { get; set; }

    public bool Damaged { get; set; }
    private const string EnemyFireTag = "EnemyFire";
    private const string FriendlyFireTag = "FriendlyFire";
    private const string LeftJoystickTag = "Left_Joystick";
    private const string RightJoystickTag = "Right_Joystick";
    private GameObject LeftJoystickGO;
    private GameObject RightJoystickGO;
    private FloatingJoystick FloatingJoystickRight;
    private FloatingJoystick FloatingJoystickLeft;
    private Vector3 movement = Vector3.zero;
    private List<Observer> observers = new List<Observer>();

    void Awake()
    {
        LeftJoystickGO = GameObject.FindGameObjectWithTag(LeftJoystickTag);
        RightJoystickGO = GameObject.FindGameObjectWithTag(RightJoystickTag);

        if (!LeftJoystickGO || !RightJoystickGO)
        {
            Debug.LogError("PlayerController requires Left_Joystick and Right_Joystick tagged objects in the scene.", this);
            enabled = false;
            return;
        }

        FloatingJoystickLeft = LeftJoystickGO.GetComponent<FloatingJoystick>();
        FloatingJoystickRight = RightJoystickGO.GetComponent<FloatingJoystick>();

        if (!FloatingJoystickLeft || !FloatingJoystickRight || !healthSlider || !m_firePoint || !m_Canvas_HUD || !m_Canvas_Attack_HUD || !m_projectile || !m_Canvas_Attack_HUD_Aim || !RespawnPos)
        {
            Debug.LogError("PlayerController is missing one or more required scene references.", this);
            enabled = false;
            return;
        }

        Player = new Player()
        {
            LeftJoystick = LeftJoystickGO.GetComponent<Joystick>(),
            RightJoystick = RightJoystickGO.GetComponent<Joystick>(),
            CurrentHealth = 100f,
            StartingHealth = 100f,
            HealthSlider = healthSlider,
            Speed = m_speed,
            PlayerRigidbody = GetComponent<Rigidbody>(),
            Movement = movement,
            PlayerTransform = transform,
            NoAimZoneRadius = 0.2f,
            FirePoint = m_firePoint,
            FirePointDistance = 1.15f,
            Projectile = m_projectile,
            FireRate = 1f,
            Canvas_HUD = m_Canvas_HUD,
            Canvas_Attack_HUD = m_Canvas_Attack_HUD,
            Offset_hud = m_Canvas_HUD.position - transform.position,
            Offset_attack_hud = m_Canvas_Attack_HUD.position - transform.position,
            ScaleSpeed = 1f,
            AimMaxScale = 10f,
            AimStartingScale = m_Canvas_Attack_HUD_Aim.localScale,
            Canvas_Attack_HUD_Aim = m_Canvas_Attack_HUD_Aim,
            OrbitRadius = 1f,
            IsDead = false,
            NormalAttack = new NormalAttack
            {
                SpeedOfAttack = 6f,
                Damage = 200f,
                Range = 8f,
                TagProjectile = FriendlyFireTag
            }
        };

        FloatingJoystickLeft.PlayerController = this;
        FloatingJoystickRight.PlayerController = this;
        FloatingJoystickRight.NoAimZoneRadius = Player.NoAimZoneRadius;
        FloatingJoystickRight.FireRate = Player.FireRate;

        new DamageUIObserver(this);
    }

    void Update()
    {
        Player.AttachedCanvasMovement();

        if (Player.IsDead && !IsRespawning)
        {
            if (GameController.gameController)
            {
                GameController.gameController.RespawnPlayer(this);
            }
            else
            {
                Debug.LogError("PlayerController cannot respawn because no GameController is available.", this);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Player.LJoyStickAct && !Player.IsDead)
        {
            Player.MovementControl();
        }

        if (shoot)
        {
            if (!Player.IsDead)
            {
                Player.Shoot();
            }

            shoot = false;
        }
    }

    private void OnGUI()
    {
        NotifyAllObservers();
        Damaged = false;

        if (Player.RJoyStickAct)
        {
            Player.Aiming();
        }
        else if (!Player.RJoyStickAct)
        {
            Player.NotAiming();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(EnemyFireTag))
        {
            Player.TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>().NormalAttack.Damage);
            Damaged |= !Player.IsDead;
        }
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
