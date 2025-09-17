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
    // Update is called once per frame
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
        myAnimator.SetTrigger("attack");
        ApplyDamageToEnemies();
        Invoke("ResetAttackTrigger", 1f);
    }

    void AirAttack()
    {
        myAnimator.SetTrigger("air_attack");
        ApplyDamageToEnemies();
        Invoke("ResetAttackTrigger", 0.5f);
    }
    void CrouchAttack()
    {
        isAttacking = true;
        myAnimator.SetTrigger("crouch_attack");
        ApplyDamageToEnemies();
        Invoke("ResetAttackTrigger", 0.5f);
    }

    void ResetAttackTrigger()
    {
        myAnimator.ResetTrigger("attack");
        myAnimator.ResetTrigger("air_attack");
        myAnimator.ResetTrigger("crouch_attack");
        isAttacking = false;
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
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, AttackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(AttackDamage);
            }
        }
    }
}
