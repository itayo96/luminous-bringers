using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerController : PlayerController
{
    // Public Arrow Members
    public GameObject arrowToRight, arrowToLeft;

    // Private Arrow Members
    private float arrowWaitTime;
    private float xArrowPosition = 0.7f;

    private bool isArrowInAir = false;

    // --------
    // Starters
    // --------
    new void Start()
    {
        base.Start();

        arrowWaitTime = base.leftClickAnimTime - 0.33f;
    }
    
    // -----------
    // Controllers
    // -----------
    protected override void LeftClick()
    {
        Shoot(arrowToRight, arrowToLeft, arrowWaitTime);
    }

    protected override void RightClick()
    {
        Shoot(arrowToRight, arrowToLeft, arrowWaitTime);
    }
    
    void Shoot(GameObject arrowToRight, GameObject arrowToLeft, float waitTime)
    {
        // Coroutine of shooting to wait a little before creating the arrow (so the animation would get to the right frame)
        if (base.isFacingRight)
        {
            StartCoroutine(OnShoot(arrowToRight, xArrowPosition, waitTime));
        }
        else
        {
            StartCoroutine(OnShoot(arrowToLeft, (-1) * xArrowPosition, waitTime));
        }
    }

    // ------
    // Events
    // ------
    IEnumerator OnShoot(GameObject arrow, float x_position, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Vector2 arrowPosition = transform.position;

        arrowPosition += new Vector2(x_position, 0f);
        Instantiate(arrow, arrowPosition, Quaternion.identity);
    }
}
