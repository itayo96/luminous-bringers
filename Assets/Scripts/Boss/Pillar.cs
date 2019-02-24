using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public float startingY = -5.10f;
    public float endingY = -3.3f;

    private bool playerGotHit = false;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, startingY, 0);
        StartCoroutine(OnSummon());
    }

    IEnumerator OnSummon()
    {
        yield return new WaitForSeconds(0.1f);

        // Going Up
        while (transform.position.y <= endingY)
        {
            transform.position += new Vector3(0, 0.25f, 0);
            yield return new WaitForSeconds(0.02f);
        }

        playerGotHit = true; // Avoid colliding while disappearing
        yield return new WaitForSeconds(0.5f);

        // Disappering
        Destroy(gameObject.GetComponent<Collider2D>(), 0);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        while (sr.color.a > 0)
        {
            sr.color = new Color(255f, 255f, 255f, sr.color.a - 0.2f);
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Colliding with Player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !playerGotHit)
        {
            collision.gameObject.GetComponent<PlayerController>().OnGettingHit();
            playerGotHit = true;
        }
    }
}
