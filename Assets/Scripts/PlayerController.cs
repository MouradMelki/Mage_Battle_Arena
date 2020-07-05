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

    public bool Damaged { get; set; }
    private readonly string enemyFire = "EnemyFire";
    private GameObject LeftJoystickGO;
    private GameObject RightJoystickGO;
    private FloatingJoystick FloatingJoystickRight;
    private FloatingJoystick FloatingJoystickLeft;
    private Vector3 movement = Vector3.zero;
    private List<Observer> observers = new List<Observer>();

    void Awake()
    {
        LeftJoystickGO = GameObject.FindGameObjectWithTag("Left_Joystick");
        RightJoystickGO = GameObject.FindGameObjectWithTag("Right_Joystick");

        FloatingJoystickLeft = LeftJoystickGO.GetComponent<FloatingJoystick>();
        FloatingJoystickRight = RightJoystickGO.GetComponent<FloatingJoystick>();

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
                TagProjectile = "FriendlyFire"
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

        if (Player.IsDead)
        {
            GameController.gameController.RespawnPlayer(this);
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
            Player.Shoot();
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
        if (collision.gameObject.tag.Equals(enemyFire))
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
