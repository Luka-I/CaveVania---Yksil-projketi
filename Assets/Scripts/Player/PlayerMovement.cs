using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator myAnimator;

    private PlayerCombat playerCombat;

    private bool facingRight = true;

    public float speed = 2.0f;
    public float horizMovement;

    public bool crouching;

    // Start is called before the first frame update
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    public bool IsCrouching()
    {
        return crouching;
    }

    // Update is called once per frame
    private void Update()
    {
        horizMovement = Input.GetAxis("Horizontal");
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        // Disable movement if the player is attacking
        if (!playerCombat.isAttacking)
        {
            if (!crouching)
            {
                // Instantly set velocity based on horizontal input and max speed
                rb2D.velocity = new Vector2(horizMovement * speed, rb2D.velocity.y);
            }
            else
            {
                // If crouching, stop movement
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            }

            Flip(horizMovement); // Handle character flipping
            myAnimator.SetFloat("speed", Mathf.Abs(horizMovement));
        }
        else
        {
            // If attacking, freeze movement
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }
    }

    private void Flip (float horizontal)
    {
        if (horizontal < 0 && facingRight || horizontal > 0 && !facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            crouching = true;
            myAnimator.SetBool("crouched", crouching);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            crouching = false;
            myAnimator.SetBool("crouched", crouching);
        }
    }
}
