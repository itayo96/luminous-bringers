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
    public float timeout = 3f;

    // Private Text Members
    private string[] sentences;
    private int index;
    private float lastRefreshTime = 0f;

    Coroutine typeCorutine = null;

    // --------
    // Starters
    // --------
    void Start()
    {
        sentences = null;
        index = 0;
    }

    // -------------------
    // Updaters & Checkers
    // -------------------
    void Update()
    {
        if (timeout > 0 && continueButton.activeSelf && Time.time >= lastRefreshTime + timeout)
        {
            OnNextSentence();
        }
    }

    // -----------
    // Controllers
    // -----------
    public void Display(string[] instructions)
    {
        // Set player input to disabled, to stop it from moving while reading
        if (player != null)
        {
            player.OnInputEnabling(false);
        }

        // Setup for typing text
        textDisplay.text = "";
        sentences = null;
        sentences = instructions;
        index = 0;

        lastRefreshTime = Time.time;

        // Stop typing if overriding
        if (typeCorutine != null)
        {
            StopCoroutine(typeCorutine);
        }

        typeCorutine = StartCoroutine(Type());
    }

    public void Display(string instruction)
    {
        // Setup for typing text
        textDisplay.text = "";
        sentences = null;
        sentences = new string[1];
        sentences[0] = instruction;
        index = 0;

        lastRefreshTime = Time.time;

        // Stop typing if overriding
        if (typeCorutine != null)
        {
            StopCoroutine(typeCorutine);
        }

        typeCorutine = StartCoroutine(Type());
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

        lastRefreshTime = Time.time;

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
            if (player != null)
            {
                player.OnInputEnabling(true);
            }
        }
    }
}
