using System.Collections;
using System.Collections.Generic;
using QuantumTek.MenuSystem;
using UnityEngine;

public class VictoryScript : MonoBehaviour
{
    // Components
    public Window victoryWindow;

    // ------
    // Events
    // ------
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController.isInputEnabled = false;
            ChiefController.isActive = false;
            
            victoryWindow.Open();
        }
    }
}
