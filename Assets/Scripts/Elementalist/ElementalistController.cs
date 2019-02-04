using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalistController : PlayerController
{
    // Public Stats Members
    public int manaMaxCount = 3;
    public float manaRefreshRate = 5f;
    public Texture2D manaTexture;

    // Private Stats Members
    private int mana = 3;
    private float nextManaRefresh = 0.0f;

    // Public ElementalBall Members
    public GameObject fireBallToRight, fireBallToLeft;
    public GameObject iceBallToRight, iceBallToLeft;

    // Private ElementalBall Members
    private float fireBallWaitTime;
    private float iceBallWaitTime;
    private float xBallPosition = 0.7f;

    new void Start()
    {
        base.Start();

        mana = manaMaxCount;

        fireBallWaitTime = base.leftClickAnimTime - 0.33f;
        iceBallWaitTime = base.rightClickAnimTime - 0.33f;
    }

    // -----------
    // Controllers
    // -----------
    protected override void LeftClick()
    {
        Cast(fireBallToRight, fireBallToLeft, fireBallWaitTime);
    }

    protected override void RightClick()
    {
        Cast(iceBallToRight, iceBallToLeft, iceBallWaitTime);
    }

    protected override void Refresh()
    {
        // Refresh mana if needed and after set period of time
        if ((Time.time > nextManaRefresh) && (mana < manaMaxCount))
        {
            nextManaRefresh = Time.time + manaRefreshRate;
            mana++;
        }
    }

    void Cast(GameObject elementalToRight, GameObject elementalToLeft, float waitTime)
    {
        // If can cast
        if (mana > 0)
        {
            mana--;

            // If first time casting, put the refresh time from this point
            if (nextManaRefresh == 0)
            {
                nextManaRefresh = Time.time + manaRefreshRate;
            }

            // Coroutine of casting to wait a little before creating the elemental ball (so the animation would get to the right frame)
            if (base.isFacingRight)
            {
                StartCoroutine(OnCast(elementalToRight, xBallPosition, waitTime));
            }
            else
            {
                StartCoroutine(OnCast(elementalToLeft, (-1) * xBallPosition, waitTime));
            }    
        }
    }

    IEnumerator OnCast(GameObject elemental, float x_position, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Vector2 elementalPosition = transform.position;

        elementalPosition += new Vector2(x_position, 0f);
        Instantiate(elemental, elementalPosition, Quaternion.identity);
    }

    // ---
    // GUI
    // ---
    protected override void OnGUI()
    {
        base.OnGUI();

        // Mana
        Rect manaIcon = new Rect(Screen.width - 100, 10, 70, 108);
        for (int i = 1; i <= mana; i++)
        {
            GUI.DrawTexture(manaIcon, manaTexture);
            manaIcon.x -= (manaIcon.width + 10);
        }
    }
}
