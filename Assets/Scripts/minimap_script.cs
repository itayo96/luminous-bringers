using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimap_script : MonoBehaviour
{
    
    public Transform targetTransform;
    Vector3 tempVec3 = new Vector3();

    void LateUpdate() {
        // Find a better way to do this
        if (targetTransform.position.x > 152)
        {
            tempVec3.x = 152 + 40;
        }
        else
        {
            tempVec3.x = targetTransform.position.x + 40;
        }

        tempVec3.y = this.transform.position.y;
        tempVec3.z = this.transform.position.z;
        this.transform.position = tempVec3;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
