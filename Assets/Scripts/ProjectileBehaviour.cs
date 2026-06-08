using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private Vector3 startingPosition;
    private float rangeSqr;
    private bool configured;

    public float Damage { get; private set; }

    private void Awake()
    {
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!configured)
        {
            return;
        }

        Vector3 movedDistance = transform.position - startingPosition;
        if (movedDistance.sqrMagnitude > rangeSqr)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    public void Configure(NormalAttack attack)
    {
        Damage = attack.Damage;
        rangeSqr = attack.Range * attack.Range;
        startingPosition = transform.position;
        configured = true;
    }
}
