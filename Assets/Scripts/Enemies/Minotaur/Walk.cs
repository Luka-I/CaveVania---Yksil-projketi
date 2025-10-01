using UnityEngine;

public class Walk : StateMachineBehaviour
{
    public float speed = 2.5f;

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

        if (minotaur.IsPlayerInAttackRange())
        {
            minotaur.TryAttack();
        }
        else
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Slash_Attack");
        animator.ResetTrigger("Thrust_Attack");
        animator.ResetTrigger("Throw_Attack");
    }
}