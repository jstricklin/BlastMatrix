using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Project.Utilities;

public class TankCanvasController : MonoBehaviour
{
    Canvas canvas;
    public TextMeshProUGUI playerLabel;
    Transform toFace;
    Camera mainCam;
    

    void Start()
    {
        canvas = GetComponent<Canvas>();
        toFace = transform;
        mainCam = Camera.main;
    }

    // public void SetPlayerName(string name)
    // {
    //     playerLabel.text = name;
    // }

    void LateUpdate()
    {
        toFace.FaceCamera(mainCam);
    }
}
