using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator myAnimator;
    public ParticleSystem deathParticles;
    public ParticleSystem hitParticles;

    public int maxHealth = 100;
    int currentHealth;
    private bool isDead = false;

    public float invincibilityDuration = 0.2f;
    private bool isInvincible = false;

    public bool hasDeathAnimation = false;

    // NEW: Rock platform reference
    public GameObject rockPlatform; // Assign the rock platform that should be enabled/disabled

    // Flash effect variables
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;

    // Rock group system
    private List<GameObject> rockGroup;
    private MinotaurFight minotaurFight;

    void Start()
    {
        currentHealth = maxHealth;

        // Get SpriteRenderer for flash effect
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // NEW: Disable the rock platform at start
        if (rockPlatform != null)
        {
            rockPlatform.SetActive(false);
        }
    }

    // NEW: Set rock group reference
    public void SetRockGroup(List<GameObject> group, MinotaurFight fight)
    {
        rockGroup = group;
        minotaurFight = fight;
    }

    public void SetAsRock()
    {
        maxHealth = 1;
        currentHealth = 1;
        hasDeathAnimation = false;
    }

    // NEW: Method to identify this as a rock
    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        StartCoroutine(InvincibilityCoroutine());

        // Flash red and play hit particles when damaged but not killed
        if (currentHealth > 0)
        {
            StartCoroutine(FlashRed());
            PlayHitParticles();
        }

        // Handle rock destruction - check if it has RockGroup component
        RockGroup rockGroup = GetComponent<RockGroup>();
        if (rockGroup != null && currentHealth <= 0)
        {
            DestroyAllRocks();
            return;
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    // NEW: Enable rock platform when enemy dies
    private void EnableRockPlatform()
    {
        if (rockPlatform != null)
        {
            rockPlatform.SetActive(true);
            Debug.Log("Rock platform enabled!");
        }
    }

    // Coroutine to flash the enemy red when hit
    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    // Play hit particles when damaged but not killed
    void PlayHitParticles()
    {
        if (hitParticles != null)
        {
            ParticleSystem particles = Instantiate(hitParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }
    }

    private void DestroyAllRocks()
    {
        if (isDead) return;
        isDead = true;

        // Use the normal Die method for this rock (which includes particles)
        DieForRock();

        // Find and destroy all other rocks
        RockGroup[] allRocks = FindObjectsOfType<RockGroup>();
        foreach (RockGroup rock in allRocks)
        {
            if (rock != null && rock.gameObject != this.gameObject)
            {
                Enemy rockEnemy = rock.GetComponent<Enemy>();
                if (rockEnemy != null && !rockEnemy.isDead)
                {
                    rockEnemy.DieForRock(); // Use Die method for particles
                }
            }
        }
    }

    // NEW: Special Die method for rocks that skips enemy-specific logic
    private void DieForRock()
    {
        Debug.Log("Rock destroyed with particles!");

        // Play death particles if available
        if (deathParticles != null)
        {
            PlayDeathParticles();
        }

        // Destroy immediately (skip all the enemy-specific death logic)
        Destroy(gameObject);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void DisappearAfterDeath()
    {
        // NEW: Enable rock platform before destroying
        EnableRockPlatform();
        Destroy(gameObject);
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        // NEW: Enable rock platform when enemy dies
        EnableRockPlatform();

        // Don't run normal enemy death for rocks
        if (GetComponent<MinotaurRock>() != null)
        {
            Debug.Log("Skipping normal death for rock");
            return;
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
            // NEW: Enable rock platform before immediate destruction
            EnableRockPlatform();
            Destroy(gameObject);
        }

        GetComponent<Collider2D>().enabled = false;
    }

    float GetDeathAnimationLength()
    {
        RuntimeAnimatorController ac = myAnimator.runtimeAnimatorController;

        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name.ToLower().Contains("death") || clip.name.ToLower().Contains("dead"))
            {
                return clip.length;
            }
        }

        Debug.LogWarning("Death animation not found, using default 2 second duration");
        return 2f;
    }

    void PlayDeathParticles()
    {
        ParticleSystem particles = Instantiate(deathParticles, transform.position, Quaternion.identity);
        particles.Play();
        Destroy(particles.gameObject, particles.main.duration);
    }
}