using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float FirePointDistance              { get; set; }
    public float OrbitRadius                    { get; set; }
    public float NoAimZoneRadius                { get; set; }
    public float StartingHealth                 { get; set; }
    public float CurrentHealth                  { get; set; }
    public float SpeedMovement                  { get; private set; }
    public float SpeedAimHUD                    { get; private set; }
    public float LerpSpeedHUDAtt                { get; private set; } // m_speed in player stats
    public float ZAimPosition                   { get; private set; }
    public float AimAngle                       { get; private set; }
    public float Speed                          { get; set; }
    public float ScaleSpeed                     { get; set; }
    public float FireRate                       { get; set; }
    public float AimMaxScale                    { get; set; }
    public bool IsDead                          { get; set; }
    public bool RJoyStickAct                    { get; set; }
    public bool LJoyStickAct                    { get; set; }
    public Quaternion NewRotation               { get; private set; }
    public Vector3 Movement                     { get; set; }
    public Vector3 Offset_attack_hud            { get; set; }
    public Vector3 Offset_hud                   { get; set; }
    public Vector3 AimStartingScale             { get; set; }
    public Rigidbody Canvas_Attack_HUD          { get; set; }
    public Rigidbody PlayerRigidbody            { get; set; }
    public Transform Canvas_HUD                 { get; set; }
    public Transform FirePoint                  { get; set; }
    public Transform PlayerTransform            { get; set; }
    public RectTransform Canvas_Attack_HUD_Aim  { get; set; }
    public Slider HealthSlider                  { get; set; }
    public Image DamageImage                    { get; private set; }
    public Joystick LeftJoystick                { get; set; }
    public Joystick RightJoystick               { get; set; }
    public NormalAttack NormalAttack            { get; set; }
    public GameObject Projectile                { get; set; }

    public Player()
    {

    }

    public void Shoot()
    {
        float aimAngle = (float)(Mathf.PI * PlayerRigidbody.rotation.eulerAngles.y / 180.0);
        if (NormalAttack.Direction.magnitude < NoAimZoneRadius)
        {
            NormalAttack.Direction = new Vector3(Mathf.Sin(aimAngle), 0f, Mathf.Cos(aimAngle));
            NormalAttack.Direction = AimAutomaticallyAtEnemy(NormalAttack.Direction, PlayerTransform, NormalAttack.Range + OrbitRadius);
        }
        FirePoint.position = PlayerTransform.position + NormalAttack.Direction.normalized * FirePointDistance;
        GameObject friendlyFire = Instantiate(Projectile, FirePoint.position, Quaternion.identity) as GameObject;
        friendlyFire.GetComponent<Rigidbody>().velocity = NormalAttack.Direction.normalized * NormalAttack.SpeedOfAttack;
        friendlyFire.tag = NormalAttack.TagProjectile;
        friendlyFire.GetComponent<ProjectileBehaviour>().NormalAttack = NormalAttack;
        PlayerRigidbody.MoveRotation(Quaternion.LookRotation(NormalAttack.Direction));
    }

    protected Vector3 AimAutomaticallyAtEnemy(Vector3 movement, Transform transform, float range)
    {
        Collider[] enemiesInLookRange;
        Vector3 enemyPosToPlayer;
        LayerMask EnemyLayerMask = LayerMask.GetMask("Enemy");
        enemiesInLookRange = Physics.OverlapSphere(transform.position, range, EnemyLayerMask);
        if (enemiesInLookRange.Length > 0)
        {
            float closest = range;
            foreach (Collider enemy in enemiesInLookRange)
            {
                enemyPosToPlayer = enemy.transform.position - transform.position;
                if (enemyPosToPlayer.magnitude < closest)
                {
                    closest = enemyPosToPlayer.magnitude;
                    movement = enemyPosToPlayer;
                }
            }
        }
        return movement;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        HealthSlider.value = CurrentHealth;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            IsDead = true;
        }
    }

    /*
     * Movement controls of the player
     */
    public void MovementControl()
    {
        Movement = new Vector3(LeftJoystick.Horizontal, 0f, LeftJoystick.Vertical).normalized * Speed * Time.deltaTime;
        PlayerRigidbody.MovePosition(PlayerTransform.position + Movement);
        PlayerRigidbody.MoveRotation(Quaternion.LookRotation(new Vector3(LeftJoystick.Horizontal, 0f, LeftJoystick.Vertical)));
    }

    public void Aiming()
    {
        float zAimRotation = Mathf.Atan2(RightJoystick.Horizontal, RightJoystick.Vertical) * Mathf.Rad2Deg;
        float zAimPosition = (Canvas_Attack_HUD_Aim.localScale.y / 2f) + OrbitRadius;
        Canvas_Attack_HUD_Aim.localScale = new Vector3(Canvas_Attack_HUD_Aim.localScale.x,
                                                       Mathf.Lerp(Canvas_Attack_HUD_Aim.localScale.y, AimMaxScale, ScaleSpeed * Time.deltaTime),
                                                       Canvas_Attack_HUD_Aim.localScale.z);

        Canvas_Attack_HUD_Aim.localPosition = new Vector3(zAimPosition * Mathf.Sin((float)(Mathf.PI * Canvas_Attack_HUD.rotation.y / 180.0)),
                                                          zAimPosition * Mathf.Cos((float)(Mathf.PI * Canvas_Attack_HUD.rotation.y / 180.0)),
                                                          0f);
        
        Quaternion newRotation = Quaternion.Euler(90.0f, zAimRotation, 0.0f);
        Canvas_Attack_HUD.MoveRotation(newRotation);
    }

    public void NotAiming()
    {
        bool reset = Canvas_Attack_HUD_Aim.localScale != AimStartingScale;
        if (reset)
        {
            Canvas_Attack_HUD_Aim.localScale = AimStartingScale;
            Canvas_Attack_HUD_Aim.position = Canvas_Attack_HUD.position;
        }
    }

    public void AttachedCanvasMovement()
    {
        Canvas_Attack_HUD.position = PlayerTransform.position + Offset_attack_hud;
        Canvas_HUD.position = PlayerTransform.position + Offset_hud;
        Canvas_HUD.rotation = Quaternion.LookRotation(new Vector3(0f, -90f, 0f));
    }
}
