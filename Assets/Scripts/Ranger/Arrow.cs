using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Public Movement Members
    public float velocityX = 20f;
    public static int arrowsCountInAir = 0;

    // Private Movement Members
    private float velocityY = 4f;
    private Rigidbody2D rigidBody;
    private bool grounded = false;

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
        if (!grounded)
        {
            rigidBody.velocity = new Vector2(velocityX, velocityY-=0.5f);
            rigidBody.AddRelativeForce(new Vector2(1,0) * 200);

            Vector2 velocity = rigidBody.velocity;
            float combinedVelocity = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);
            float angle = Mathf.Atan2(velocity.y, velocity.x) * 180 / Mathf.PI;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
        }
        else
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(255f, 255f, 255f, sr.color.a - 0.02f);
        }
    }

    void FixedUpdate() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI") ||
            collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
        {
            arrowsCountInAir--;

            grounded = true;
            Destroy(gameObject.GetComponent<Rigidbody2D>(), 0);
            Destroy(gameObject.GetComponent<Collider2D>(), 0);
            Destroy(gameObject, 1.5f);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().GotHitByArrow();
            Destroy(gameObject, 0);
        }
    }
}
