using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public List<Camera> cameras;

    private int activeCameraIndex;

    // Start is called before the first frame update
    void Start()
    {
        activeCameraIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            activeCameraIndex++;
            activeCameraIndex %= cameras.Count;
            
            switchCamera();
        }
    }

    void switchCamera()
    {
         PlayerController.isInputEnabled = (activeCameraIndex == 0);
         FreeViewCameraScript.isActive = (activeCameraIndex == 1);

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].enabled = (activeCameraIndex == i);
        }
    }
}
