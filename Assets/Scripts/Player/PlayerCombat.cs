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
    private bool damageApplied = false;

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
                if (thePlayerJump.grounded)
                {
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
        damageApplied = false;
        myAnimator.SetTrigger("attack");
        Invoke("ResetAttackTrigger", 1f);
    }

    void AirAttack()
    {
        damageApplied = false;
        myAnimator.SetTrigger("air_attack");
        Invoke("ResetAttackTrigger", 0.5f);
    }

    void CrouchAttack()
    {
        isAttacking = true;
        damageApplied = false;
        myAnimator.SetTrigger("crouch_attack");
        Invoke("ResetAttackTrigger", 0.5f);
    }

    // Animation Event Methods
    public void OnAttackApex()
    {
        if (!damageApplied)
        {
            ApplyDamageToEnemies();
            damageApplied = true;
        }
    }

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
        damageApplied = false;
    }

    private void OnDrawGizmos()
    {
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
        Debug.Log("ApplyDamageToEnemies called");

        foreach (Transform attackPoint in AttackPoints)
        {
            if (attackPoint == null) continue;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, AttackRange, enemyLayers);
            Debug.Log($" - Found {hitEnemies.Length} colliders in attack range");

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    Debug.Log($" - Hit: {enemy.gameObject.name} on layer {enemy.gameObject.layer}");

                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        Debug.Log($" - Calling TakeDamage on {enemy.gameObject.name}");
                        enemyComponent.TakeDamage(AttackDamage);
                    }
                    else
                    {
                        Debug.Log($" - No Enemy component found on {enemy.gameObject.name}");
                    }
                }
            }
        }
    }
}