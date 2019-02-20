using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBall : MonoBehaviour
{
    // Public Movement Members
    public float velocityX = 5f;

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
    }

    void Awake() { }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update()
    {
        rigidBody.velocity = new Vector2(velocityX, velocityY);
        Destroy(gameObject, destroyAfter);
    }

    void FixedUpdate() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI") ||
            collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
        {
            Destroy(gameObject, 0);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().GotHitByElementalBall(gameObject.tag);
            Destroy(gameObject, 0);
        }
    }
}
