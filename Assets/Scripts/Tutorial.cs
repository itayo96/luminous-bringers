using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    enum StateMachine
    {
        waiting_for_a = 0,
        waiting_for_d,
        waiting_for_jump,
        waiting_for_warlord_slash,
        waiting_for_warlord_block,
        waiting_for_switch_to_elemental,
        waiting_for_elementalist_fire,
        waiting_for_elementalist_ice,
        waiting_for_switch_to_ranger,
        waiting_for_ranger_shot,
        waiting_for_ranger_haste,
        waiting_for_first_c,
        waiting_for_second_c,
        waiting_for_escape,
        exit,

        count
    }

    [System.Serializable]
    public struct MultidimensionalString
    {
        public string[] strings;
    }

    // Public Objects
    public GameObject warlord, elementalist, ranger;
    public Dialog dialog;

    // Public Instructions
    public MultidimensionalString[] keyInstructions = new MultidimensionalString[(int)StateMachine.count];

    // Private State Machine
    private StateMachine state;
    private bool shouldWrite = true;

    private KeyCode[] codes = {
        KeyCode.A, KeyCode.D, KeyCode.Space,
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.LeftShift,
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.LeftShift,
        KeyCode.Mouse0, KeyCode.Mouse1,
        KeyCode.C, KeyCode.C,
        KeyCode.Break, KeyCode.Break
    };

    // Start is called before the first frame update
    void Start()
    {
        state = StateMachine.waiting_for_a;
        dialog.timeout = 0;

        warlord.SetActive(true);
        elementalist.SetActive(false);
        ranger.SetActive(false);

        dialog.player = warlord.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        WaitForKey(codes[(int)state], keyInstructions[(int)state].strings);

        switch (state)
        {
            case StateMachine.waiting_for_a:
            case StateMachine.waiting_for_d:
            case StateMachine.waiting_for_jump:
                break;

            case StateMachine.waiting_for_warlord_slash:
            case StateMachine.waiting_for_warlord_block:
            case StateMachine.waiting_for_switch_to_elemental:
                warlord.SetActive(true);
                elementalist.SetActive(false);
                ranger.SetActive(false);
                dialog.player = warlord.GetComponent<PlayerController>();
                break;

            case StateMachine.waiting_for_elementalist_fire:
            case StateMachine.waiting_for_elementalist_ice:
            case StateMachine.waiting_for_switch_to_ranger:
                warlord.SetActive(false);
                elementalist.SetActive(true);
                ranger.SetActive(false);
                dialog.player = elementalist.GetComponent<PlayerController>();
                break;

            case StateMachine.waiting_for_ranger_shot:
            case StateMachine.waiting_for_ranger_haste:
                warlord.SetActive(false);
                elementalist.SetActive(false);
                ranger.SetActive(true);
                dialog.player = ranger.GetComponent<PlayerController>();
                break;

            case StateMachine.waiting_for_first_c:
            case StateMachine.waiting_for_second_c:
                break;

            case StateMachine.waiting_for_escape:
                if (dialog.textDisplay.text == "" && Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (warlord.activeSelf)
                    {
                        warlord.SetActive(false);
                        elementalist.SetActive(true);
                        ranger.SetActive(false);
                    }
                    else if (elementalist.activeSelf)
                    {
                        warlord.SetActive(false);
                        elementalist.SetActive(false);
                        ranger.SetActive(true);
                    }
                    else if (ranger.activeSelf)
                    {
                        warlord.SetActive(true);
                        elementalist.SetActive(false);
                        ranger.SetActive(false);
                    }
                }
                break;

            case StateMachine.exit:
            default:
                break;
        }

        if (dialog.textDisplay.text == "" && Input.GetKeyDown(KeyCode.H))
        {
            shouldWrite = true;
        }
    }

    void WaitForKey(KeyCode key, string[] instructions)
    {
        if (shouldWrite)
        {
            dialog.Display(instructions);
        }
        shouldWrite = false;

        if (dialog.textDisplay.text == "" && Input.GetKeyDown(key) && state != StateMachine.exit)
        {
            shouldWrite = true;
            state++;
        }
    }
}
