/* 
 * mayra barrera, 2017
 * script for stroke control + log
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ExperimentControl : MonoBehaviour {

    private string currentDrawingObject;   
    private Dictionary<string, GameObject> DrawnObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> VisibleSmartGuide = new Dictionary<string, bool>();

    private GameObject SmartGuidesUI;
    private GameObject StrokeContainer;
    private GameObject UIContainer;

    private GameObject UserHead;
    private GameObject ControllerRight;
    private GameObject ControllerLeft;

    private void Start()
    {
        //for test, remove in study
        //here you can manually set each guide and see how it works (it also allows to bypass the set scene)
        #region test     
        /*GlobalVars.Instance.currentParticipant = "0000";
        GlobalVars.Instance.currentStudy = 2;
        GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.seeModel;


        GlobalVars.Instance.thisRotationType = GlobalVars.RotationType.walk;
        GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.SmartGuides;
        GlobalVars.Instance.thisSmartGuide = GlobalVars.SmartGuide.grid;

        /*EventManager.Instance.TriggerEvent(new CreateXMLEvent(false, "", GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy));
        EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString()},
            GlobalVars.LogCategory.startTry, "went to view object"));*/
        #endregion

        GlobalVars.Instance.thisObjectShape = GlobalVars.ObjectShape.shape2;

        //to show the rotation type
        SmartGuidesUI = GameObject.Find("SmartGuides").gameObject;
        SmartGuidesUI.SetActive(true);

        StrokeContainer = GameObject.Find("StrokeContainer");

        UIContainer = GameObject.Find("UIContainer");
        DrawnObjects = new Dictionary<string, GameObject>();

        foreach (Transform child in UIContainer.transform)
        {
            DrawnObjects.Add(child.name, child.gameObject);
            child.gameObject.SetActive(false);
        }

        foreach (GlobalVars.SmartGuide guide in (GlobalVars.SmartGuide[])Enum.GetValues(typeof(GlobalVars.SmartGuide)))
        {
            if (guide != GlobalVars.SmartGuide.none)
            {
                VisibleSmartGuide.Add(guide.ToString(), false);
            }
        }

        OnSceneLoad();
    }

    private void OnSceneLoad()
    {
        GlobalVars.Instance.thisSmartGuide = GlobalVars.SmartGuide.grid;
        GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.SmartGuides;

        //show current drawing object
        currentDrawingObject = GlobalVars.Instance.thisObjectShape.ToString();
       // DrawnObjects[currentDrawingObject].SetActive(true);

        //activate selected SmartGuide
        switch (GlobalVars.Instance.thisSmartGuide)
        {
            case GlobalVars.SmartGuide.none:
                Debug.Log("no smartguide activated");
                break;

            default:
                VisibleSmartGuide[GlobalVars.Instance.thisSmartGuide.ToString()] = true;
                break;
        }
    }

    public void Phase(int strokeCount)
    {
        if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing)
        {
            /*EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
                GlobalVars.LogCategory.startTry, "start try"));*/

            //start user position log
            UserHead = GameObject.Find("Camera");
            StartCoroutine("LogUserPose", 0.5F);
        }

        if (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.finish)
        {
            /*EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
                GlobalVars.LogCategory.endTry, "finish try_" + strokeCount));*/

            //end user position log
            StopCoroutine("LogUserPose");

            //ACTIVATE IN STUDY
            #region save objects //commented
            
            //take screen capture of object
            EventManager.Instance.TriggerEvent(new TakeScreenshotEvent(GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(),GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() }, "Camera_X", 512, 512));

            EventManager.Instance.TriggerEvent(new TakeScreenshotEvent(GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy,
               new string[] { GlobalVars.Instance.thisObjectShape.ToString(),GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() }, "Camera_Y", 512, 512));

            EventManager.Instance.TriggerEvent(new TakeScreenshotEvent(GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(),GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() }, "Camera_Z", 512, 512));

            EventManager.Instance.TriggerEvent(new TakeScreenshotEvent(GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(),GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() }, "Camera_Perspective", 512, 512));

            //save stroke meshFilter
            foreach (Transform child in StrokeContainer.transform)
            {
                MeshFilter mf = child.GetComponent<MeshFilter>() as MeshFilter;
                Mesh mesh = mf.sharedMesh;

                /*EventManager.Instance.TriggerEvent(new SaveStrokeEvent(GlobalVars.Instance.currentParticipant, GlobalVars.Instance.currentStudy,
                   new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() }, child.name, mesh));*/
            }   
            
            #endregion

            //load start scene
            SceneManager.LoadScene("startScene", LoadSceneMode.Single);
        }
    }

    public void StartStroke(Vector3 position)
    {
        /*EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
            GlobalVars.LogCategory.newStroke, "" + position));*/

        //DrawnObjects[currentDrawingObject].SetActive(false);
    }

    public void EndStroke(Vector3 position)
    {
        //end user position log
        /*EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
            new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
            GlobalVars.LogCategory.endStroke, "" + position));*/

        
        //DrawnObjects[currentDrawingObject].SetActive(true);
    }

    IEnumerator LogUserPose(float waitTime)
    {
        while (true)
        {
            //start log of controller position
            if (ControllerRight == null)
            {
                ControllerRight = GameObject.Find("Controller (right)");
            }

            if (ControllerLeft == null)
            {
                ControllerLeft = GameObject.Find("Controller (left)");
            }

            //USER HEAD POSE
            #region head
            string position = "" + UserHead.transform.position.x + "," + UserHead.transform.position.y + "," + UserHead.transform.position.z;

            Quaternion rotation = UserHead.transform.rotation;
            string rotationString = "" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;

            string viewNormal = UserHead.transform.forward.x + "," + UserHead.transform.forward.y + "," + UserHead.transform.forward.z;

            string pose = position + "/" + rotation + "/" + viewNormal;

            /*EventManager.Instance.QueueEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
                GlobalVars.LogCategory.userPose, pose));*/
            #endregion

            //RIGHT CONTROLLER
            #region rightController
            position = "" + ControllerRight.transform.position.x + "," + ControllerRight.transform.position.y + "," + ControllerRight.transform.position.z;

            rotation = ControllerRight.transform.rotation;
            rotationString = "" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;

            viewNormal = ControllerRight.transform.forward.x + "," + ControllerRight.transform.forward.y + "," + ControllerRight.transform.forward.z;

            pose = position + "/" + rotation + "/" + viewNormal;

            /*EventManager.Instance.QueueEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
                GlobalVars.LogCategory.rightControllerPose, pose));*/
            #endregion

            //LEFT CONTROLLER
            #region leftController
            position = "" + ControllerLeft.transform.position.x + "," + ControllerLeft.transform.position.y + "," + ControllerLeft.transform.position.z;

            rotation = ControllerLeft.transform.rotation;
            rotationString = "" + rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;

            viewNormal = ControllerLeft.transform.forward.x + "," + ControllerLeft.transform.forward.y + "," + ControllerLeft.transform.forward.z;

            pose = position + "/" + rotation + "/" + viewNormal;

            /*EventManager.Instance.QueueEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
                new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisVisualGuide.ToString(), GlobalVars.Instance.thisSmartGuide.ToString() },
                GlobalVars.LogCategory.leftControllerPose, pose));*/
            #endregion

            yield return new WaitForSeconds(waitTime);
        }
    }
}
