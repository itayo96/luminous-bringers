using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBall : MonoBehaviour
{
    // Public Movement Members
    public float velocityX = 5f;

    // Private Movement Members
    private float velocityY = 0;
    private Rigidbody2D rigidBody;

    // Public Collision Members
    public string counterNameTag;

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
    }

    void Awake() { }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update()
    {
        rigidBody.velocity = new Vector2(velocityX, velocityY);
    }

    void FixedUpdate() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Colliding with UI
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI") ||
            collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
        {
            Destroy(gameObject, 0);
        }

        // Colliding with Player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().OnGettingHit();
            Destroy(gameObject, 0);
        }

        // Colliding with another cast ball
        if (collision.gameObject.layer == LayerMask.NameToLayer("ElementalBall"))
        {
            string colTag = collision.gameObject.tag;
            Destroy(collision.gameObject, 0);
            
            // Colliding with counter
            if (colTag == counterNameTag)
            {
                Destroy(gameObject, 0);
            }
        }
    }
}
