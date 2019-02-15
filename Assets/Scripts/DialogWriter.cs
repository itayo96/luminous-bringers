using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogWriter : MonoBehaviour
{
    // Components
    public Dialog dialog;

    // Public Text Members
    public string[] instructions;

    // ------
    // Events
    // ------
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            dialog.Display(instructions);
            Destroy(gameObject);
        }
    }
}
