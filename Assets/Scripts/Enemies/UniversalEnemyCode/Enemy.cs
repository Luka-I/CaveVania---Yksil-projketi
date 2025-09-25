using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator myAnimator;
    public ParticleSystem deathParticles; // Reference to the death particle system

    public int maxHealth = 100;
    int currentHealth;
    private bool isDead = false;

    public float invincibilityDuration = 0.2f;
    private bool isInvincible = false;

    public bool hasDeathAnimation = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0)
        {
            isDead = true;
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

        // Play death particles if available
        if (deathParticles != null)
        {
            PlayDeathParticles();
        }

        if (hasDeathAnimation && myAnimator != null)
        {
            myAnimator.SetBool("dead", true);
        }

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        if (!hasDeathAnimation)
        {
            Destroy(gameObject);
        }
        else
        {
            Invoke("DisappearAfterDeath", 1.0f);
        }
    }

    void PlayDeathParticles()
    {
        // Create a copy of the particle system at the enemy's position
        ParticleSystem particles = Instantiate(deathParticles, transform.position, Quaternion.identity);

        // Play the particles
        particles.Play();

        // Destroy the particle system after it finishes playing
        Destroy(particles.gameObject, particles.main.duration);
    }
}