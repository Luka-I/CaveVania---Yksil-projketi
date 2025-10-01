using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinotaurRock : MonoBehaviour
{
    [Header("Rock Settings")]
    public int damage = 15;
    public float speed = 8f;
    public float knockbackForce = 3f;
    public float lifeTime = 4f;

    [Header("Collision Settings")]
    public LayerMask environmentLayers;

    private Vector2 direction;
    private Rigidbody2D rb;
    private bool isDestroyed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Make sure we have a rigidbody
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Set rigidbody properties
        rb.gravityScale = 0f; // No gravity for now
        rb.drag = 0f;

        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 throwDirection, float rockSpeed)
    {
        direction = throwDirection.normalized;
        speed = rockSpeed;

        // Apply force immediately in Start
        if (rb != null)
        {
            rb.velocity = direction * speed;
            Debug.Log($"Rock launched with velocity: {rb.velocity}");
        }
    }

    void Update()
    {
        // Optional: Keep moving in direction if needed
        if (rb != null && rb.velocity.magnitude < speed * 0.5f)
        {
            rb.velocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        Debug.Log($"Rock hit: {collision.gameObject.name}");

        // Ignore other rocks
        if (collision.GetComponent<MinotaurRock>() != null)
            return;

        // Damage player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
                Debug.Log("Rock dealt damage to player!");
            }
            DestroyRock();
        }
        // Destroy on environment
        else if (IsEnvironment(collision))
        {
            Debug.Log("Rock hit environment");
            DestroyRock();
        }
    }

    private bool IsEnvironment(Collider2D collision)
    {
        return environmentLayers == (environmentLayers | (1 << collision.gameObject.layer));
    }

    private void DestroyRock()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        Destroy(gameObject);
    }
}