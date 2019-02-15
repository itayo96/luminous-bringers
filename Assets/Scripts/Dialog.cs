using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    // Components
    public TextMeshProUGUI textDisplay;
    public GameObject continueButton;
    public PlayerController player;

    // Public Text Members
    public float typingSpeed = 0.02f;

    // Private Text Members
    private string[] sentences;
    private int index;

    // --------
    // Starters
    // --------
    void Start()
    {
        sentences = null;
        index = 0;
    }

    // -----------
    // Controllers
    // -----------
    public void Display(string[] instructions)
    {
        // Set player input to disabled, to stop it from moving while reading
        player.OnInputEnabling(false);

        // Setup for typing text
        textDisplay.text = "";
        sentences = instructions;
        index = 0;

        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        // A delayed text concating that uses sleep for a smooth letter by letter typing
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // On last sentence the button should be with "end", rest of times is "continue"
        if (index == sentences.Length - 1)
        {
            continueButton.GetComponent<TextMeshProUGUI>().text = "End";
        }
        else
        {
            continueButton.GetComponent<TextMeshProUGUI>().text = "Continue";
        }

        // Set the button game object to active, thus clickable
        continueButton.SetActive(true);
    }

    // ------
    // Events
    // ------
    public void OnNextSentence()
    {
        // Setting continue button to false after clicking it
        continueButton.SetActive(false);

        // Display next sentence if there is one, else finish
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            continueButton.SetActive(false);

            // Enable back the player's input checking
            player.OnInputEnabling(true);
        }
    }
}
