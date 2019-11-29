using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers 
{
    public static void FaceCamera(this Transform toFace, Camera mainCam)
    {
        toFace.transform.LookAt(toFace.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up);

    }
}
