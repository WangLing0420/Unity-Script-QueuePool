using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        EnemyMove();
    }

    private void EnemyMove()
    {
        rb.position += new Vector3(0, 0, -1) * speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            EnemyPool.Instance.ReturnEnemy(this);
        }
    }
}
