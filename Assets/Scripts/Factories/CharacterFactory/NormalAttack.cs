using UnityEngine;
using System.Collections;

public class NormalAttack : MonoBehaviour
{
    public float SpeedOfAttack { get; set; }
    public float Range { get; set; }
    public float AttackRange { get; private set; }
    public float Damage { get; set; }
    public string TagProjectile { get; set; }
    public GameObject Projectile { get; private set; }
    public Vector3 StartingScale { get; private set; }
    public Vector3 Direction { get; set; }
    public Vector3 StartingPosition { get; private set; }
    public Vector3 MovedDistance { get; private set; }
    public Rigidbody AimRigidbody { get; private set; }
}
