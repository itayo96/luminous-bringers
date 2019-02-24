using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarlordController : PlayerController
{
    // Public Attack Members
    public Transform attackPosition;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public float damageWaitTime = 0.40f;

    // -----------
    // Controllers
    // -----------
    protected override void LeftClick()
    {
        // Check for nearby enemies
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemies);
        ArrayList enemies = new ArrayList();

        for (int i = 0; i < colliders.Length; i++)
        {
            // If this enemy was already damaged, do nothing
            if (enemies.Contains(colliders[i].gameObject))
            {
                continue;
            }

            // Slash the enemy
            StartCoroutine(OnDamagingEnemy(colliders[i].gameObject));

            // Add to damaged enemies
            enemies.Add(colliders[i].gameObject);
        }
    }

    protected override void RightClick() { }

    // ------
    // Events
    // ------
    IEnumerator OnDamagingEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(damageWaitTime);

        enemy.GetComponent<EnemyController>().GotSlashedBySword();
    }

    public override void OnGettingHit()
    {
        // If blocking, do not get hit
        if (!IsBlocking())
        {
            base.OnGettingHit();
        }
    }

    // ------
    // Stats
    // ------
    protected override void GotHit()
    {
        // If blocking, do not get hit
        if (!animator.GetBool("IsRightClick"))
        {
            base.GotHit();
        }
    }

    public bool IsBlocking()
    {
        return animator.GetBool("IsRightClick");
    }

    // ---
    // GUI
    // ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
