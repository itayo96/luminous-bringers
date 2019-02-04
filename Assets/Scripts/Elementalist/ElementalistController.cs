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

    new void Start()
    {
        base.Start();
        mana = manaMaxCount;
    }

    // -----------
    // Controllers
    // -----------
    protected override void LeftClick() { }

    protected override void RightClick() { }

    protected override void Refresh() { }

    // ---
    // GUI
    // ---
    protected override void OnGUI()
    {
        base.OnGUI();

        // Mana
        Rect manaIcon = new Rect(Screen.width - 100, 10, 64, 64);
        for (int i = 1; i <= mana; i++)
        {
            GUI.DrawTexture(manaIcon, manaTexture);
            manaIcon.x -= (manaIcon.width + 10);
        }
    }
}
