using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Walk : StateMachineBehaviour
{
    public float speed = 2.5f;
    // Remove the fixed attackRange - we'll get it from MinotaurFight

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

        // Check if player is within ANY attack range
        if (minotaur.IsPlayerInAttackRange())
        {
            minotaur.TryAttack();
        }
        else
        {
            // Continue moving towards player
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset all triggers when exiting the state to prevent lingering triggers
        animator.ResetTrigger("Slash_Attack");
        animator.ResetTrigger("Thrust_Attack");
        animator.ResetTrigger("Throw_Attack");
    }
}
