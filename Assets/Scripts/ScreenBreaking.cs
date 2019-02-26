using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBreaking : MonoBehaviour
{
    public GameObject originalWhiteScreen;
    public GameObject activateAfterBreak;
    public GameObject[] pieces;
    
    void Start()
    {
        StartCoroutine(Break());
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(11f);

        foreach (GameObject img in pieces)
        {
            img.SetActive(true);

            Rigidbody2D rb = img.AddComponent<Rigidbody2D>();
            rb.mass = Random.Range(15f, 35f);
            rb.rotation = Random.Range(-90f, 90f);

            RectTransform rt = img.GetComponent<RectTransform>();
            rt.rotation = Quaternion.Euler(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
        }

        yield return new WaitForSeconds(2f);

        Destroy(originalWhiteScreen);

        foreach (GameObject img in pieces)
        {
            Rigidbody2D rb = img.GetComponent<Rigidbody2D>();
            rb.gravityScale = Random.Range(100f, 150f);
            rb.angularDrag = Random.Range(0f, 100f);
            rb.angularVelocity = 3;
        }

        activateAfterBreak.SetActive(true);

        for (int i = 0; i < 100; i++)
        {
            for (int img = 0; img < pieces.Length; img++)
            {
                int x = (img % 2 == 0) ? 1 : -1;

                RectTransform rt = pieces[img].GetComponent<RectTransform>();
                Vector3 vec = rt.rotation.eulerAngles + new Vector3(x * 3f, x * 3f, x * 3f);
                rt.rotation = Quaternion.Euler(vec);
            }
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(2f);

        foreach (GameObject img in pieces)
        {
            Destroy(img);
        }

        Destroy(gameObject);
    }
}
