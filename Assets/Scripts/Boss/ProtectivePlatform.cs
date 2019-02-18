using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProtectivePlatform : MonoBehaviour
{
    public UnityEvent onPlayerCollision;

    private LineRenderer lineRenderer;
    public Transform lineCeiling;

    private bool wasUsed = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!wasUsed && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            onPlayerCollision.Invoke();
            wasUsed = true;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
            lineCeiling.position = hit.point;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, lineCeiling.position);
        }
    }
}
