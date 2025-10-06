using UnityEngine;

public class Walk : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float closeCombatRange = 2f; // NEW: Distance where minotaur prefers melee

    Transform player;
    Rigidbody2D rb;
    MinotaurFight minotaur;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        minotaur = animator.GetComponent<MinotaurFight>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        minotaur.LookAtPlayer();

        float distanceToPlayer = Vector2.Distance(rb.position, player.position);

        // NEW: Check if we should close distance for melee
        bool shouldCloseDistance = ShouldCloseDistanceForMelee();

        if (minotaur.IsPlayerInAttackRange() && !shouldCloseDistance)
        {
            // Player is in range and we don't need to close distance, so attack
            minotaur.TryAttack();
        }
        else
        {
            // Either player is out of range OR we need to close distance for melee
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            // NEW: Optional - if we're actively closing distance, we can attack once we're close enough
            if (shouldCloseDistance && distanceToPlayer <= closeCombatRange)
            {
                minotaur.TryAttack();
            }
        }
    }

    // NEW: Determine if minotaur should close distance for melee
    private bool ShouldCloseDistanceForMelee()
    {
        // Check if throw attack is on cooldown and player is too far for melee
        float distanceToPlayer = Vector2.Distance(rb.position, player.position);

        // If throw is on cooldown and player is outside melee range, close distance
        if (IsThrowOnCooldown() && distanceToPlayer > closeCombatRange)
        {
            return true;
        }

        return false;
    }

    // NEW: Check if throw attack is on cooldown
    private bool IsThrowOnCooldown()
    {
        // Access the minotaur's throw cooldown (you'll need to make this public or add a public method)
        // If you can't access it directly, we'll need to add a public method to MinotaurFight
        return minotaur.IsThrowOnCooldown();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Slash_Attack");
        animator.ResetTrigger("Thrust_Attack");
        animator.ResetTrigger("Throw_Attack");
    }
}