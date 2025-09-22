using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator myAnimator;

    public int maxHealth = 100;
    int currentHealth;
    private bool isDead = false; // Track if the enemy is dead

    public float invincibilityDuration = 0.2f; // Adjust as needed
    private bool isInvincible = false;

    public bool hasDeathAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return; // Also check if already dead

        currentHealth -= damage;
        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0)
        {
            isDead = true; // Mark as dead
            Die();
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void DisappearAfterDeath()
    {
        Destroy(gameObject);
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        if (hasDeathAnimation && myAnimator != null)
        {
            myAnimator.SetBool("dead", true); // Trigger death animation if available
        }        

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;


        if (!hasDeathAnimation)
        {
            Destroy(gameObject); // Instantly destroy if no death animation
        }
        else
        {
            Invoke("DisappearAfterDeath", 1.0f); // Wait for a second before disappearing (adjust as needed)
        }
    }
}

