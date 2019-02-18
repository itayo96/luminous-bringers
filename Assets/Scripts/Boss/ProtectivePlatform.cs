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
            wasUsed = true;
            onPlayerCollision.Invoke();

            StartCoroutine(GoUp(collision.gameObject));
        }
    }

    IEnumerator GoUp(GameObject collidie)
    {
        yield return new WaitForSeconds(0.3f);

        // Disable any Y movement for the player
        collidie.GetComponent<Rigidbody2D>().gravityScale = 0;
        collidie.GetComponent<Rigidbody2D>().constraints |= RigidbodyConstraints2D.FreezePositionY;
        Destroy(collidie.GetComponent<BoxCollider2D>());

        // Move up the platform and the player for a little
        float originalY = transform.position.y;
        float originalCeilingY = lineCeiling.transform.position.y;
        float originalCollideY = collidie.transform.position.y;
        while (collidie.transform.position.y < (originalY + originalCeilingY) / 2f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            collidie.transform.position = new Vector3(collidie.transform.position.x, collidie.transform.position.y + 0.1f, collidie.transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }

        // Cast ray from the playform to the ceiling
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, new Vector2(transform.position.x, transform.position.y + 0.5f));
        lineRenderer.SetPosition(1, new Vector2(transform.position.x, transform.position.y + 0.5f));

        // Animate the ray
        while (lineRenderer.GetPosition(1).y < lineCeiling.position.y)
        {
            lineRenderer.SetPosition(1, new Vector3(lineRenderer.GetPosition(1).x, lineRenderer.GetPosition(1).y + 0.1f, lineRenderer.GetPosition(1).z));
            yield return new WaitForSeconds(0.03f);
        }

        // Move up the player for a little more
        originalCollideY = collidie.transform.position.y;
        originalCeilingY = lineCeiling.transform.position.y;
        collidie.transform.position = new Vector3(transform.position.x, collidie.transform.position.y, collidie.transform.position.z);
        while (collidie.transform.position.y < (originalCollideY + originalCeilingY) / 2f)
        {
            collidie.transform.position = new Vector3(collidie.transform.position.x, collidie.transform.position.y + 0.1f, collidie.transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
