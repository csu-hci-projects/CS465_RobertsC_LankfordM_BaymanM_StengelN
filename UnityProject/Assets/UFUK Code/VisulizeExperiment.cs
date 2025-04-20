/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisulizeExperiment : MonoBehaviour
{

    private TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void updateText()
    {

        string textToWrite = "Participant Number =" + " " + GlobalVars.Instance.participantNumber + "\n" +
            "Shape =" + " " + GlobalVars.Instance.thisObjectShape + "\n" +
            "Guide =" + " " + GlobalVars.Instance.thisVisualGuide + "\n" +
            "Size =" + " " + GlobalVars.Instance.thisDrawnSize + "\n" +
            "Direction =" + " " + GlobalVars.Instance.thisDrawnDirection + "\n" +
            "Finished =" + " " + GlobalVars.Instance.finishedConditionNumber + "/36 \n" +
            "ID= " + " " + GlobalVars.Instance.drawinID;
        textMesh.text= textToWrite;

    }


}*/
