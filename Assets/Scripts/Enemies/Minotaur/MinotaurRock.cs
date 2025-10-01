using System.Collections;
using System.Collections.Generic;
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
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 throwDirection, float rockSpeed)
    {
        direction = throwDirection.normalized;
        speed = rockSpeed;

        // Apply force immediately
        if (rb != null)
        {
            rb.velocity = direction * speed;
            float randomTorque = Random.Range(-50f, 50f);
            rb.AddTorque(randomTorque);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

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