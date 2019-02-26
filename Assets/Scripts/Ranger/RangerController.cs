using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerController : PlayerController
{
    // Public Arrow Members
    public GameObject arrowToRight, arrowToLeft;
    
    // Private Arrow Members
    private float arrowWaitTime;
    private float xArrowPosition = 0.6f;
    private bool isArrowInAir = false;

    // Public Haste Members
    public float hasteBonus = 10f;
    public float hasteDuration = 3f;
    public float hasteCooldown = 12f;
    public Texture2D hasteTexture;

    // Private Haste Members
    private bool isHasteActive = false;

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
        Haste();
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

    void Haste()
    {
        if (isHasteActive)
        {
            return;
        }

        runSpeed += hasteBonus;

        isHasteActive = true;

        StartCoroutine(OnHaste());
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

    IEnumerator OnHaste()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        while (GetComponent<SpriteRenderer>().color.r > 0.2f)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r - 0.1f, 1f, GetComponent<SpriteRenderer>().color.b - 0.1f);
            yield return new WaitForSeconds(0.04f);
        }

        while (GetComponent<SpriteRenderer>().color.r < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r + 0.1f, 1f, GetComponent<SpriteRenderer>().color.b + 0.1f);
            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(hasteDuration - 0.64f);

        runSpeed -= hasteBonus;

        yield return new WaitForSeconds(hasteCooldown);

        isHasteActive = false;
    }

    // ---
    // GUI
    // ---
    protected override void OnGUI()
    {
        if (!isInputEnabled)
        {
            return;
        }

        base.OnGUI();

        // Haste Icon
        if (!isHasteActive)
        {
            Rect hasteIcon = new Rect(240, 93, 25, 25);
            GUI.DrawTexture(hasteIcon, hasteTexture);
        }
    }
}
