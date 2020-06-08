﻿using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private GameObject gunImpact;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor")) // We display impact on floor
            Destroy(Instantiate(gunImpact, collision.contacts[0].point + (Vector3.up * 0.001f), Quaternion.identity), 3f);
        Destroy(gameObject);
    }
}