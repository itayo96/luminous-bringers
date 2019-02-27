using System.Collections;
using System.Collections.Generic;
using QuantumTek.MenuSystem;
using UnityEngine;

public class VictoryScript : MonoBehaviour
{
    // Components
    public Window victoryWindow;
    public AudioSource audioSource;

    // ------
    // Events
    // ------
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController.isInputEnabled = false;
            ChiefController.isActive = false;

            AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource audioS in allAudioSources)
            {
                audioS.Stop();
            }
            audioSource.Play();
            victoryWindow.Open();
        }
    }
}
