using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableObject : MonoBehaviour, IShootable
{
    [SerializeField]
    float impulseMultiplier = 100f;

    [SerializeField]
    float rotationMultiplier = 25f;

    public void OnHit(bool hitByPlayer, Transform transform)
    {
        Vector3 direction = (gameObject.transform.position - transform.position).normalized;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(direction * impulseMultiplier, ForceMode.Impulse);
        rb.AddTorque(direction * rotationMultiplier);
    }

}
