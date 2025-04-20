using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterfaceControl : MonoBehaviour {

    //script to control the experiment, I had it in a different scene, where I manually set all the values with the participants data (including the guides condition)

    //private bool firstTime;

    void Start()
    {
        //firstTime = true;

        GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.none;
        GlobalVars.Instance.thisSmartGuide = GlobalVars.SmartGuide.none;
    }

    public void SetParticipant(string text)
    {
        //Debug.Log(text);
        GlobalVars.Instance.currentParticipant = text;
    }

    public void SetStudy(string text)
    {
        //Debug.Log(text);
        GlobalVars.Instance.currentStudy = int.Parse(text);
    }

    public void SetVisualization(int option)
    {
        //Debug.Log(option);
        GlobalVars.Instance.thisVisualGuide = (GlobalVars.VisualGuide)option;
    }

    public void SetSmartGuide(int option)
    {
        //Debug.Log(option);
        GlobalVars.Instance.thisSmartGuide = (GlobalVars.SmartGuide)option;
    }

    public void SetRotation(int option)
    {
        //Debug.Log(option);
        GlobalVars.Instance.thisRotationType = (GlobalVars.RotationType)option;
    }

    public void SetObjectType(int option)
    {
        //Debug.Log(option);
        GlobalVars.Instance.thisObjectShape = (GlobalVars.ObjectShape)option;
    }

    public void StartButtonClicked()
    {
        //start xml for log
       /* if (firstTime)
        {
            EventManager.Instance.TriggerEvent(new CreateXMLEvent(false,"", 
                GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy));
            firstTime = false;
        }

        //save experiment options
        EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant,GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
            GlobalVars.LogCategory.startTry, "went to view object"));*/

        //init experiment
        GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.seeModel;

        Debug.Log(GlobalVars.Instance.thisVisualGuide);
        Debug.Log(GlobalVars.Instance.thisSmartGuide);
        Debug.Log(GlobalVars.Instance.thisObjectShape);
        Debug.Log(GlobalVars.Instance.thisRotationType);

        //load scene
        switch (GlobalVars.Instance.currentStudy)
        {
            case 1:
                SceneManager.LoadScene("study1_visualGuides", LoadSceneMode.Single);
                break;

            case 2:
                SceneManager.LoadScene("study2_SmartUsability", LoadSceneMode.Single);
                break;

            case 3:
                SceneManager.LoadScene("study3_SmartGuides", LoadSceneMode.Single);
                break;

            default:
                Debug.Log("scene not exist" + GlobalVars.Instance.currentStudy);
                break;
        }        
    }

    public void EndButtonClicked()
    {
        EventManager.Instance.TriggerEvent(new EndLogEvent());
    }
}
