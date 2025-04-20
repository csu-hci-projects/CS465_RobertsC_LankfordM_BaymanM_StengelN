using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{
    public GameObject cube;
    public GameObject controller;
    bool isScale = false;
    public float scaleFactor = 1;
    float x = 1.0f;
    Vector3 tempScale;
    //bool isTempScale = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(cube.transform.position, controller.transform.position);
        print("distance: " + distance);
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("test Touch: A Button (Down)");
            isScale = !isScale;
            if (isScale)
            {
                if (distance == 0) { distance = 0.1f; }
                x=cube.transform.localScale.x/(scaleFactor * distance);
            }            
        }
        if (isScale)
        {
            cube.transform.localScale = Vector3.one * x * scaleFactor * distance;
        }
    }
}
