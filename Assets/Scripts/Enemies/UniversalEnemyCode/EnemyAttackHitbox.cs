using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 1;
    public Transform enemy;
    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false; // Reset per attack
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasHit && other.CompareTag("Player"))
        {
            hasHit = true;
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector2 direction = (other.transform.position - enemy.position).normalized;
                playerHealth.TakeDamage(damage, direction);
            }
        }
    }
}
