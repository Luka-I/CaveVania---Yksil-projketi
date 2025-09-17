using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerJump : MonoBehaviour
{
    [Header("Public Vars")]
    public float jumpForce;
    public bool grounded;
    private Rigidbody2D rb;
    private Animator myAnimator;

    [Header("Private Vars")]
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float radOCircle;
    [SerializeField] private LayerMask whatIsGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        myAnimator.SetBool("jumping", !grounded);
        grounded = Physics2D.OverlapCircle(groundcheck.position,radOCircle,whatIsGround);
        if (Input.GetButtonDown("Jump") && grounded)
        {            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);           
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundcheck.position, radOCircle);
    }

}
