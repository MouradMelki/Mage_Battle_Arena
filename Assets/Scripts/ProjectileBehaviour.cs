using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private Vector3 Direction;
    private Vector3 startingPosition;
    private Vector3 movedDistance;
    private Rigidbody aimRigidbody;
    private GameObject Player;
    public NormalAttack NormalAttack { get; set; }

    private void Awake()
    {
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        movedDistance = transform.position - startingPosition;
        if (movedDistance.magnitude > NormalAttack.Range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
