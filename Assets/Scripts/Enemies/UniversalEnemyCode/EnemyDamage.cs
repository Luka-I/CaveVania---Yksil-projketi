using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    public int damage;
    public float knockbackForce = 5f;

    // Removed the playerHealth reference from Start since we'll get it from the collision

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the PlayerHealth component from the player GameObject
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Calculate knockback direction
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                // Call TakeDamage on the player
                playerHealth.TakeDamage(damage, knockbackDirection);
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on player object!");
            }
        }
    }
}