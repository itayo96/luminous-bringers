using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainTrigger : MonoBehaviour
{
    public PatrollingEnemeyController enemy;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            enemy.OnEnteringDomain(collision.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            enemy.OnEnteringDomain(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            enemy.OnExitingDomain();
        }
    }
}
