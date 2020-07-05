using UnityEngine;
using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class EnemyBot : MonoBehaviour
{
    public float FireRate               { get; set; }
    public float FirePointDistance      { get; set; }
    public float OrbitRadius            { get; set; }
    public float CurrentHealth          { get; set; }
    public float StartingHealth         { get; set; }
    public bool IsDead                  { get; set; }
    public Transform FirePoint          { get; set; }
    public Transform EnemyTransform     { get; set; }
    public GameObject Projectile        { get; set; }
    public GameObject Player            { get; set; }
    public NavMeshAgent Nav             { get; set; }
    public Slider HealthSlider          { get; set; }
    public NormalAttack NormalAttack    { get; set; }
    public Rigidbody EnemyRigidbody     { get; set; }

    public void Shoot()
    {
        if (!IsDead)
        {
            Collider[] enemiesInLookRange;
            Vector3 enemyPosToPlayer;
            LayerMask TeamLayerMask = LayerMask.GetMask("Player");// | LayerMask.GetMask("Team");
            float closest = NormalAttack.Range + OrbitRadius;
            enemiesInLookRange = Physics.OverlapSphere(EnemyTransform.position, closest, TeamLayerMask);
            if (enemiesInLookRange.Length > 0)
            {
                foreach (Collider enemy in enemiesInLookRange)
                {
                    enemyPosToPlayer = enemy.transform.position - EnemyTransform.position;
                    if (enemyPosToPlayer.magnitude < closest)
                    {
                        closest = enemyPosToPlayer.magnitude;
                        NormalAttack.Direction = enemyPosToPlayer;
                        FirePoint.position = EnemyTransform.transform.position + NormalAttack.Direction.normalized * FirePointDistance;
                        GameObject enemyFire = Instantiate(Projectile, FirePoint.position, Quaternion.identity) as GameObject;
                        enemyFire.GetComponent<Rigidbody>().velocity = NormalAttack.Direction.normalized * NormalAttack.SpeedOfAttack;
                        enemyFire.tag = NormalAttack.TagProjectile;
                        enemyFire.GetComponent<ProjectileBehaviour>().NormalAttack = NormalAttack;
                    }
                }
            }
        }
        EnemyRigidbody.MoveRotation(Quaternion.LookRotation(NormalAttack.Direction));
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

    public void MovementControl()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        if (Player)
        {
            if (!IsDead)
            {
                Nav.SetDestination(Player.transform.position);
            }
            else
            {
                Nav.enabled = false;
            }
        }
    }

    public void Aiming() => throw new NotImplementedException();

    public void NotAiming() => throw new NotImplementedException();

    public void AttachedCanvasMovement() => throw new NotImplementedException();
}
