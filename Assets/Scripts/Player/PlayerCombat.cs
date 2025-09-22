using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator myAnimator;
    public PlayerJump thePlayerJump;
    public PlayerController playerController;

    public Transform[] AttackPoints;
    public LayerMask enemyLayers;

    public float AttackRange = 1.0f;
    public int AttackDamage = 40;

    public float AttackRate = 2.0f;
    float nextAttackTime = 0f;

    private PlayerJump playerjump;

    public bool isAttacking;
    private bool damageApplied = false; // Track if damage has been applied for current attack

    private void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController script not found on the GameObject!");
            }
        }

        playerjump = GetComponent<PlayerJump>();

        if (playerjump == null)
        {
            Debug.LogError("playerjump script not found on the GameObject!");
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.S) && !isAttacking)
            {
                // Check if the player is grounded
                if (thePlayerJump.grounded)
                {
                    // Check if the player is crouching to trigger a crouch attack
                    if (playerController.IsCrouching() && Input.GetKey(KeyCode.S))
                    {
                        CrouchAttack();
                    }
                    else
                    {
                        GroundAttack();
                    }
                }
                else
                {
                    AirAttack();
                }
                nextAttackTime = Time.time + 1f / AttackRate;
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            isAttacking = false;
        }
    }

    void GroundAttack()
    {
        isAttacking = true;
        damageApplied = false; // Reset damage flag
        myAnimator.SetTrigger("attack");
        // Removed ApplyDamageToEnemies() call from here
        Invoke("ResetAttackTrigger", 1f);
    }

    void AirAttack()
    {
        damageApplied = false; // Reset damage flag
        myAnimator.SetTrigger("air_attack");
        // Removed ApplyDamageToEnemies() call from here
        Invoke("ResetAttackTrigger", 0.5f);
    }

    void CrouchAttack()
    {
        isAttacking = true;
        damageApplied = false; // Reset damage flag
        myAnimator.SetTrigger("crouch_attack");
        // Removed ApplyDamageToEnemies() call from here
        Invoke("ResetAttackTrigger", 0.5f);
    }

    // Animation Event Methods - Called at specific frames in animations
    public void OnAttackApex()
    {
        if (!damageApplied)
        {
            ApplyDamageToEnemies();
            damageApplied = true;
        }
    }

    // Optional: Add separate events for different attack types if needed
    public void OnAirAttackApex()
    {
        if (!damageApplied)
        {
            ApplyDamageToEnemies();
            damageApplied = true;
        }
    }

    public void OnCrouchAttackApex()
    {
        if (!damageApplied)
        {
            ApplyDamageToEnemies();
            damageApplied = true;
        }
    }

    void ResetAttackTrigger()
    {
        myAnimator.ResetTrigger("attack");
        myAnimator.ResetTrigger("air_attack");
        myAnimator.ResetTrigger("crouch_attack");
        isAttacking = false;
        damageApplied = false; // Reset for next attack
    }

    private void OnDrawGizmos()
    {
        // Draw gizmos for each attack point
        if (AttackPoints != null)
        {
            foreach (Transform attackPoint in AttackPoints)
            {
                if (attackPoint != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(attackPoint.position, AttackRange);
                }
            }
        }
    }

    void ApplyDamageToEnemies()
    {
        // Shared logic for applying damage
        foreach (Transform attackPoint in AttackPoints)
        {
            if (attackPoint == null) continue;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, AttackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                // Check if the enemy object still exists and has the Enemy component
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.TakeDamage(AttackDamage);
                    }
                }
            }
        }
    }
}