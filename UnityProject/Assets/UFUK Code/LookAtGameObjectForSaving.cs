using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtGameObjectForSaving : MonoBehaviour
{
    public Transform toLookat;

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.LookAt(toLookat);
    }
}
