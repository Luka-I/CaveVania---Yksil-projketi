using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonBehaviour : MonoBehaviour
{
    public float speed = 2f;
    public int damage;

    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float recoveryTime = 0.5f;

    public GameObject hitbox;
    private Animator anim;
    private Transform player;
    private PlayerHealth playerHealth;

    private float cooldownTimer = Mathf.Infinity;
    private float recoveryTimer = 0f;

    private enum SkeletonState { Idle, Chasing, Attacking, Recovering }
    private SkeletonState currentState = SkeletonState.Idle;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        hitbox.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        cooldownTimer += Time.deltaTime;

        switch (currentState)
        {
            case SkeletonState.Idle:
                anim.SetBool("isMoving", false);
                if (distance <= chaseRange)
                    currentState = SkeletonState.Chasing;
                break;

            case SkeletonState.Chasing:
                if (distance > chaseRange)
                {
                    currentState = SkeletonState.Idle;
                    break;
                }

                if (distance <= attackRange)
                {
                    if (cooldownTimer >= attackCooldown)
                    {
                        currentState = SkeletonState.Attacking;
                        StartAttack();
                    }
                    break;
                }

                MoveTowardPlayer();
                break;

            case SkeletonState.Attacking:
                // Wait for OnAttackAnimationEnd to handle transition to Recovering
                break;

            case SkeletonState.Recovering:
                recoveryTimer -= Time.deltaTime;
                if (recoveryTimer <= 0)
                {
                    currentState = SkeletonState.Idle;
                }
                break;
        }
    }

    private void MoveTowardPlayer()
    {
        anim.SetBool("isMoving", true);
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // Flip sprite
        if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void StartAttack()
    {
        anim.SetBool("isMoving", false);
        anim.SetTrigger("attack");
        cooldownTimer = 0;
        hitbox.SetActive(true); // Hitbox will do damage via its own trigger
    }

    // Called from Animation Event at end of attack animation
    public void OnAttackAnimationEnd()
    {
        hitbox.SetActive(false);
        currentState = SkeletonState.Recovering;
        recoveryTimer = recoveryTime;
    }

    //Toimi unity
    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }
}