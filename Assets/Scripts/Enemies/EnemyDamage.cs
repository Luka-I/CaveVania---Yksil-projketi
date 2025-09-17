using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    public int damage;
    public PlayerHealth playerHealth;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Using CompareTag for better performance
        {
            // Assuming you want to knock the player back in the opposite direction of the enemy
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized; // Calculate direction

            // Call TakeDamage and pass the knockback direction
            playerHealth.TakeDamage(damage, knockbackDirection);
        }
    }
}
