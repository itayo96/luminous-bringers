using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float startingY = 5.8f;
    public float spinSpeed = 200.0f;

    private bool grounded = false;

    void Start()
    {
        spinSpeed = Random.Range(spinSpeed - 50f, spinSpeed + 50f);
        transform.position = new Vector3(transform.position.x, startingY, 0);
    }

    void Update()
    {
        if (!grounded)
        {
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
        }
        else
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(255f, 255f, 255f, sr.color.a - 0.02f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Colliding with UI
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI") ||
            collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
        {
            grounded = true;
            Destroy(gameObject.GetComponent<Rigidbody2D>(), 0);
            Destroy(gameObject.GetComponent<Collider2D>(), 0);
            Destroy(gameObject, 1.5f);
        }

        // Colliding with Player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !grounded)
        {
            collision.gameObject.GetComponent<PlayerController>().OnGettingHit();
            Destroy(gameObject, 0);
        }
    }
}
