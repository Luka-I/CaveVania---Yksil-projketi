using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public Animator myAnimator;
    private bool isDead = false;

    public PlayerCombat playerCombat;
    public PlayerController playerMovement;
    public PlayerJump playerJump;

    private Rigidbody2D rb;

    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        playerCombat = GetComponent<PlayerCombat>();
        playerMovement = GetComponent<PlayerController>();
        playerJump = GetComponent<PlayerJump>();
        rb = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D here
    }

    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            myAnimator.SetTrigger("knockback");
            // Apply knockback if still alive
            StartCoroutine(ApplyKnockback(attackDirection));
        }
    }

    IEnumerator ApplyKnockback(Vector2 attackDirection)
    {
        // Normalize the attack direction and apply knockback force
        Vector2 knockbackDir = attackDirection.normalized * knockbackForce;

        // Set the velocity to the knockback direction
        rb.velocity = new Vector2(knockbackDir.x, rb.velocity.y); // Only knockback on the x-axis, keep y for gravity

        // Disable player movement during knockback
        playerMovement.enabled = false;

        yield return new WaitForSeconds(knockbackDuration);

        // Re-enable movement after knockback duration
        if (!isDead)
        {
            playerMovement.enabled = true;
        }
    }

    void Die()
    {
        isDead = true;
        myAnimator.SetTrigger("player_death");

        DisablePlayerActions();
        StartCoroutine(FreezeOnDeath());
    }

    void DisablePlayerActions()
    {
        if (playerCombat != null) playerCombat.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerJump != null) playerJump.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Stop any movement
            rb.isKinematic = false;     // Make sure physics still work
            rb.gravityScale = 1;        // Ensure gravity is applied
        }
    }

    IEnumerator FreezeOnDeath()
    {
        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);

        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Stop any movement
            rb.gravityScale = 0;        // Disable gravity
            rb.isKinematic = true;      // Set as kinematic to stop all physics
        }

        myAnimator.enabled = false;
    }

    bool grounded()
    {
        // Implement your grounded logic here, such as raycasting
        return playerJump != null && playerJump.grounded;
    }
}
