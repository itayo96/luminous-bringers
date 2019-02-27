using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeViewCameraScript : MonoBehaviour
{
    public float speed;
    public static bool isActive;

    private float cameraMovingSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraMovingSpeed = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (QuantumTek.MenuSystem.Menu.isPaused || !isActive)
        {
            transform.position = GameObject.Find("Main Camera").transform.position;
            return;
        }
        
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * cameraMovingSpeed);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * cameraMovingSpeed);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * cameraMovingSpeed);
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * cameraMovingSpeed);
        }
    }
    
    void LateUpdate()
    {
        if (QuantumTek.MenuSystem.Menu.isPaused || !isActive)
        {
            return;
        }
        
        var colliderBounds = GameObject.Find("Background").GetComponent<Collider2D>().bounds;
        float halfHeight = this.GetComponent<Camera>().orthographicSize;
        float halfWidth = this.GetComponent<Camera>().aspect * halfHeight;
        Vector3 tempVec3 = new Vector3();
        
        if (this.transform.position.x < colliderBounds.min.x + halfWidth)
        {
            tempVec3.x = colliderBounds.min.x + halfWidth;
        }
        else if (this.transform.position.x > colliderBounds.max.x - halfWidth)
        {
            tempVec3.x = colliderBounds.max.x - halfWidth;
        }
        else
        {
            tempVec3.x = this.transform.position.x;
        }

        if (this.transform.position.y < colliderBounds.min.y + halfHeight)
        {
            tempVec3.y = colliderBounds.min.y + halfHeight;
        }
        else if (this.transform.position.y > colliderBounds.max.y - halfHeight)
        {
            tempVec3.y = colliderBounds.max.y - halfHeight;
        }
        else
        {
            tempVec3.y = this.transform.position.y;
        }

        tempVec3.z = this.transform.position.z;
        this.transform.position = tempVec3;
    }
}
