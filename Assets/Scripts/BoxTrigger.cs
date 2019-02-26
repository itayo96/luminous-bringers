using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    public GameObject box;

    void Start()
    {
        box.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        box.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        box.SetActive(false);
    }
}
