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
    protected virtual void Start() { }
    protected virtual void Awake() { }

    // -------------------
    // Updaters & Checkers
    // -------------------
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }

    public virtual void GotSlashedBySword()
    {
        OnHit(swordHitDamage);
    }

    public virtual void GotHitByElementalBall()
    {
        OnHit(elementalHitDamage);
    }

    public virtual void GotHitByArrow()
    {
        OnHit(arrowHitDamage);
    }

    protected virtual void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // TODO: Death animation
            Destroy(gameObject);
        }
    }
}
