using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinotaurFight : MonoBehaviour
{
    public Transform player;
    public bool isFlipped = false;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float knockbackForce = 5f;
    public LayerMask playerLayer;
    private float lastAttackTime;
    private Animator animator;

    [Header("Throw Attack Settings")]
    public GameObject rockProjectilePrefab;
    public Transform throwPoint;
    public int rocksPerThrow = 3;
    public float spreadAngle = 20f;
    public float throwSpeed = 8f;

    [Header("Rock Group Settings")]
    public bool destroyAllRocksWhenOneHit = true; // ADD THIS

    private List<List<GameObject>> rockGroups = new List<List<GameObject>>(); // ADD THIS

    [System.Serializable]
    public class AttackSettings
    {
        public string attackName;
        public float triggerRange = 3f;
        public float hitboxRange = 1.5f;
        public Vector3 hitboxOffset;
        public int damage = 20;
    }

    public List<AttackSettings> attacks;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            string chosenAttack = ChooseAttack();
            if (!string.IsNullOrEmpty(chosenAttack))
            {
                animator.SetTrigger(chosenAttack);
                lastAttackTime = Time.time;
            }
        }
    }

    private string ChooseAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        List<AttackSettings> availableAttacks = attacks.FindAll(attack => distanceToPlayer <= attack.triggerRange);

        if (availableAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, availableAttacks.Count);
            return availableAttacks[randomIndex].attackName;
        }
        return null;
    }

    // Animation Events
    public void OnSlashAttack()
    {
        PerformMeleeAttack(attacks.Find(a => a.attackName == "Slash_Attack"));
    }

    public void OnThrustAttack()
    {
        PerformMeleeAttack(attacks.Find(a => a.attackName == "Thrust_Attack"));
    }

    public void OnThrowAttack()
    {
        PerformThrowAttack();
    }

    private void PerformThrowAttack()
    {
        if (rockProjectilePrefab == null || throwPoint == null) return;

        Vector2 playerDirection = (player.position - throwPoint.position).normalized;

        for (int i = 0; i < rocksPerThrow; i++)
        {
            float angleStep = spreadAngle / (rocksPerThrow - 1);
            float angleVariation = -spreadAngle / 2f + (angleStep * i);
            Vector2 throwDirection = Quaternion.Euler(0, 0, angleVariation) * playerDirection;

            GameObject rock = Instantiate(rockProjectilePrefab, throwPoint.position, Quaternion.identity);

            // The rock already has the RockGroup component on the prefab
            // No need to set anything!

            // Your existing setup code...
            Enemy enemyComponent = rock.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.SetAsRock();
            }

            MinotaurRock rockScript = rock.GetComponent<MinotaurRock>();
            if (rockScript != null)
            {
                rockScript.Launch(throwDirection, throwSpeed);
            }
        }
    }

    // Method to destroy entire rock group
    public void DestroyRockGroup(List<GameObject> rockGroup)
    {
        if (rockGroup == null) return;

        foreach (GameObject rock in rockGroup.ToArray())
        {
            if (rock != null)
            {
                Enemy enemyComponent = rock.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    // This will trigger the rock's destruction through the Enemy system
                    enemyComponent.TakeDamage(100); // Guaranteed kill
                }
            }
        }

        rockGroups.Remove(rockGroup);
    }

    // Clean up empty groups
    private void Update()
    {
        // Clean up every second
        if (Time.frameCount % 60 == 0)
        {
            rockGroups.RemoveAll(group => group.Count == 0 || group.All(rock => rock == null));
        }
    }

    private void PerformMeleeAttack(AttackSettings attack)
    {
        if (attack == null) return;

        Vector3 hitboxPosition = transform.position + transform.right * attack.hitboxOffset.x + transform.up * attack.hitboxOffset.y;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(hitboxPosition, attack.hitboxRange, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            if (playerCollider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;
                    playerHealth.TakeDamage(attack.damage, knockbackDirection * knockbackForce);
                }
            }
        }
    }

    public bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        foreach (var attack in attacks)
        {
            if (distanceToPlayer <= attack.triggerRange)
            {
                return true;
            }
        }
        return false;
    }
}