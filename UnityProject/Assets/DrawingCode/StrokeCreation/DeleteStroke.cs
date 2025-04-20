using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteStroke : MonoBehaviour
{
    public GameObject StrokeContainer;
    public void deleteStroke()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in allChildren)
        {
            childObjects.Add(child.gameObject);
        }

        Debug.Log("Deleting");
        //Debug.Log(StrokeContainer);
        Debug.Log(childObjects.Count);
        foreach (GameObject child in childObjects)
        {
            Debug.Log(child.name);
        }
        //Destroy(childObjects[1]);
        childObjects[1].SetActive(false);
    }
}
