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

    public void SetAsRock()
    {
        maxHealth = 1;
        currentHealth = 1;
        hasDeathAnimation = false;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        StartCoroutine(InvincibilityCoroutine());

        // If you want rocks to use Enemy system, you'd need different logic
        // But for now, let's keep rocks simple and not use Enemy script

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

        // Don't run normal enemy death for rocks
        MinotaurRock rock = GetComponent<MinotaurRock>();
        if (rock != null)
        {
            Debug.Log("Enemy: Rock detected in Die(), skipping normal death");
            return; // Exit early for rocks
        }

        // Only run this for regular enemies (not rocks)
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component != this && component.enabled)
            {
                component.enabled = false;
            }
        }

        // Also disable the hitbox specifically as backup
        if (TryGetComponent<SkeletonBehaviour>(out SkeletonBehaviour skeleton))
        {
            skeleton.DisableHitbox();
        }

        // Play death particles if available
        if (deathParticles != null)
        {
            PlayDeathParticles();
        }

        if (hasDeathAnimation && myAnimator != null)
        {
            myAnimator.SetBool("dead", true);
            float deathAnimationLength = GetDeathAnimationLength();
            Invoke("DisappearAfterDeath", deathAnimationLength);
        }
        else
        {
            Destroy(gameObject);
        }

        GetComponent<Collider2D>().enabled = false;
    }

    float GetDeathAnimationLength()
    {
        // Get the current AnimatorController
        RuntimeAnimatorController ac = myAnimator.runtimeAnimatorController;

        // Loop through all animation clips to find the death animation
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name.ToLower().Contains("death") || clip.name.ToLower().Contains("dead"))
            {
                return clip.length;
            }
        }

        // Fallback: if no death animation found, use 2 seconds
        Debug.LogWarning("Death animation not found, using default 2 second duration");
        return 2f;
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