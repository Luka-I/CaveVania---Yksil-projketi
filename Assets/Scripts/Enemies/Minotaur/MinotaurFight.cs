using System.Collections;
using System.Collections.Generic;
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

    [Header("Attack Cooldowns")]
    public float throwAttackCooldown = 5f;
    private float lastThrowTime = 0f;

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

                // Start throw cooldown if throw was chosen
                if (chosenAttack == "Throw_Attack")
                {
                    lastThrowTime = Time.time;
                }
            }
        }
    }

    private string ChooseAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        List<AttackSettings> availableAttacks = attacks.FindAll(attack => distanceToPlayer <= attack.triggerRange);

        if (availableAttacks.Count > 0)
        {
            // NEW: Filter out throw attack if it's on cooldown
            List<AttackSettings> validAttacks = new List<AttackSettings>();

            foreach (var attack in availableAttacks)
            {
                if (attack.attackName == "Throw_Attack")
                {
                    // Only include throw attack if it's off cooldown
                    if (!IsThrowOnCooldown())
                    {
                        validAttacks.Add(attack);
                    }
                }
                else
                {
                    // Always include melee attacks
                    validAttacks.Add(attack);
                }
            }

            // If we filtered out all attacks (only throw was available but on cooldown), 
            // use the original available attacks as fallback
            if (validAttacks.Count == 0)
            {
                validAttacks = availableAttacks;
            }

            int randomIndex = Random.Range(0, validAttacks.Count);
            return validAttacks[randomIndex].attackName;
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
        if (rockProjectilePrefab == null || throwPoint == null)
        {
            Debug.LogError("Missing rock prefab or throw point!");
            return;
        }

        Vector2 playerDirection = (player.position - throwPoint.position).normalized;

        for (int i = 0; i < rocksPerThrow; i++)
        {
            float angleStep = spreadAngle / (rocksPerThrow - 1);
            float angleVariation = -spreadAngle / 2f + (angleStep * i);
            Vector2 throwDirection = Quaternion.Euler(0, 0, angleVariation) * playerDirection;

            GameObject rock = Instantiate(rockProjectilePrefab, throwPoint.position, Quaternion.identity);
            MinotaurRock rockScript = rock.GetComponent<MinotaurRock>();

            if (rockScript != null)
            {
                rockScript.Launch(throwDirection, throwSpeed);
            }
            else
            {
                Debug.LogError("Rock prefab missing MinotaurRock script!");
            }
        }
    }

    public bool IsThrowOnCooldown()
    {
        return Time.time < lastThrowTime + throwAttackCooldown;
    }

    // NEW: Public method to get preferred attack range
    public float GetPreferredAttackRange()
    {
        // If throw is on cooldown, prefer melee range
        if (IsThrowOnCooldown())
        {
            return 2f; // Melee range
        }

        // Otherwise, use the maximum range of any available attack
        float maxRange = 0f;
        foreach (var attack in attacks)
        {
            if (attack.triggerRange > maxRange)
            {
                maxRange = attack.triggerRange;
            }
        }
        return maxRange;
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