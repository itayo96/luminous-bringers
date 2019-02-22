using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Public Movement Members
    public float velocityX = 5f;
    public static int arrowsCountInAir = 0;

    // Private Movement Members
    private float velocityY = 0;
    private Rigidbody2D rigidBody;

    // Public Destruction Members
    public float destroyAfter = 5f;

    // --------
    // Starters
    // --------
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        // Flip if fired to the left
        if (velocityX < 0)
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        arrowsCountInAir++;
    }

    void Awake() { }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update()
    {
        rigidBody.velocity = new Vector2(velocityX, velocityY);
//        
//        Vector2 v = rigidBody.velocity;
//        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
//        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void FixedUpdate() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI") ||
            collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
        {
            arrowsCountInAir--;
            Destroy(gameObject, 0);
        }
    }
}
