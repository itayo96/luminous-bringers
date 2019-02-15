using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Public Health Members
    public int health = 1;
    public int swordHitDamage = 1;
    public int elementalHitDamage = 1;
    public int arrowHitDamage = 1;

    // --------
    // Starters
    // --------
    void Start() { }
    void Awake() { }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update() { }
    void FixedUpdate() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("FireBall") || collision.gameObject.tag.Equals("IceBall"))
        {
            Destroy(collision.gameObject);

            OnHit(elementalHitDamage);
        }
        else if (collision.gameObject.tag.Equals("Arrow"))
        {
            Destroy(collision.gameObject);

            OnHit(arrowHitDamage);
        }
        else if (collision.gameObject.tag.Equals("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().OnGettingHit();
        }
    }

    public void GotSlashedBySword()
    {
        OnHit(swordHitDamage);
    }

    private void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
