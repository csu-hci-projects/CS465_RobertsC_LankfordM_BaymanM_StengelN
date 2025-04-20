using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject avatar;

    // Update is called once per frame
    void Start()
    {
        avatar = GameObject.Find("avatar"); // The player
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(avatar.transform.position.x, avatar.transform.position.y, avatar.transform.position.z);
    }

}