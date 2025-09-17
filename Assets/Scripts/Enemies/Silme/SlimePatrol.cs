using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Patrol : MonoBehaviour
{
    public GameObject PointA;
    public GameObject PointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform CurrentPoint;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        CurrentPoint = PointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = CurrentPoint.position - transform.position;
        if(CurrentPoint == PointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }

        if(Vector2.Distance(transform.position, CurrentPoint.position) < 0.5f && CurrentPoint == PointB.transform) 
        {
            flip();
            CurrentPoint = PointA.transform;            
        }
        if (Vector2.Distance(transform.position, CurrentPoint.position) < 0.5f && CurrentPoint == PointA.transform)
        {
            flip();
            CurrentPoint = PointB.transform;
        }
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void ÓnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(PointB.transform.position, 0.5f);
    }
}
