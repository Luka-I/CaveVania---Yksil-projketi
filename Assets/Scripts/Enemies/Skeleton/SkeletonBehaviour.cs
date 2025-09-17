using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonBehaviour : MonoBehaviour
{
    [SerializeField] private float attackCoolDown;
    [SerializeField] private float chaseRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject hitbox;

    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private PlayerHealth playerHealth;
    private Transform player;
    private bool isChasing;
    private bool isAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        hitbox.gameObject.SetActive(false);
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (isAttacking)
        {
            // Stop the chase if attacking
            anim.SetBool("isMoving", false);
            return;
        }

        // Check if the player is within chase range
        if (PlayerInChaseRange())
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            anim.SetBool("isMoving", false); // Stop moving if out of chase range
        }

        // Attack if within range and cooldown allows
        if (PlayerInAttackRange() && cooldownTimer >= attackCoolDown)
        {
            AttackPlayer();
        }
    }

    private bool PlayerInChaseRange()
    {
        return Vector2.Distance(transform.position, player.position) <= chaseRange;
    }

    private bool PlayerInAttackRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private void ChasePlayer()
    {
        if (!isAttacking) // Only chase if not attacking
        {
            anim.SetBool("isMoving", true); // Play walking animation
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            // Flip sprite to face player
            if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        hitbox.gameObject.SetActive(true);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        // Only apply damage if the player is within range when the attack starts
        if (PlayerInAttackRange() && playerHealth != null)
        {
            Vector2 attackDirection = (player.position - transform.position).normalized;
            playerHealth.TakeDamage(damage, attackDirection);
        }
    }

    // Called by the animation event at the end of the attack animation
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        hitbox.gameObject.SetActive(false);
    }
}