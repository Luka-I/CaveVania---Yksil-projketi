using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatFly : MonoBehaviour
{
    public float speed = 5f;  // Speed of the bat
    public float lifetime = 10f;  // How long the bat stays alive before despawning

    private Rigidbody2D rb;
    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D
        animator = GetComponent<Animator>();
    }
    public void ActivateBat()
    {
        gameObject.SetActive(true); // Enable the bat
        animator.SetBool("isFlying", true);
        rb.velocity = new Vector2(-speed, 0f); // Adjust direction and speed
        StartCoroutine(DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
