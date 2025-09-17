using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatFly : MonoBehaviour
{
    public float speed = 5f;  // Speed of the bat
    public float lifetime = 10f;  // How long the bat stays alive before despawning
    private bool isFlying = false;

    private Rigidbody2D rb;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D
        animator = GetComponent<Animator>();
    }
    public void ActivateBat()
    {
        gameObject.SetActive(true); // Enable the bat
        isFlying = true;            // Set it flying
        animator.SetBool("isFlying", true);
        rb.velocity = new Vector2(-speed, 0f); // Adjust direction and speed
        Invoke("DisableBat", lifetime); // Schedule disabling after the lifetime
    }
    void DisableBat()
    {
        isFlying = false;
        rb.velocity = Vector2.zero; // Stop movement
        animator.SetBool("isFlying", false);
        gameObject.SetActive(false); // Deactivate the bat instead of destroying it
    }
}
