/* 
 * mayra barrera, 2021
 * script for drawing control
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using System;
using TMPro;

public class DrawController : MonoBehaviour
{
    public string _name;
    
    private InputDevice rightHandDevice;
    private bool lastButtonState = false;
    //private bool lastButtonState2 = false;

    public bool _drawing;
    public GameObject brush;
    public static GameObject rightBrush;

    public bool tempState=false;

    //private bool movedFoward = false;
    public GameObject n0 = null;

    public Vector3 tempPosition = new Vector3(100, 100, 100);
    public Vector3 tempLocalPosition = new Vector3(100, 100, 100);
    float timeSinceLastCall;

    public bool toggleTranslating = false;
    public bool toggleRotating = false;
    public Vector3 offsetTranslating = Vector3.positiveInfinity;
    public Vector3 offsetRotating = Vector3.positiveInfinity;

    public DeleteStroke ds;
    public GameObject EventManagerObj;
    public GameObject MenuCanvasObj;
    //public GameObject CameraRigObj;
    public List<int> selectQueue = new List<int>();

    public void Start()
    {
        //get info for drawing
        _name = gameObject.name;
        _drawing = false;

        brush = transform.Find("Brush").gameObject;
        rightBrush = brush;

        GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.drawing;
    }

    //public void onEnable()
    //{
    //    //grab right hand controllers
    //    var rightHandDevices = new List<InputDevice>();
    //    InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

    //    if (rightHandDevices.Count == 1)
    //    {
    //        rightHandDevice = rightHandDevices[0];
    //        Debug.Log(string.Format("Device name '{0}' with role '{1}'", rightHandDevice.name, rightHandDevice.role.ToString()));
    //    }
    //    else if (rightHandDevices.Count > 1)
    //    {
    //        Debug.Log("Found more than one left hand!");
    //    } else
    //    {
    //        Debug.Log("No Device found!");

    //    }
    //}

    public void Update()
    {
        timeSinceLastCall += UnityEngine.Time.deltaTime;
        //send position for drawing
        EventManager.Instance.QueueEvent(new OnDrawingEvent(brush.transform));

        //check if trigger pressed
        //bool triggerValue1 = false;
        //bool tempState1 = rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out triggerValue1) // did get a value
        //                && triggerValue1; // the value we got

        //if state of the button changed since last frame
        if (tempState != lastButtonState) 
        {
            OnTriggerClick(tempState);
            lastButtonState = tempState;
        }

        if (tempState != lastButtonState)
        {
            OnTriggerClick(tempState);
            lastButtonState = tempState;
        }

        /*
        //this is the instruction to finish the try and store all the strokes as prefabs
        if (Input.GetKeyDown(KeyCode.N))
        {
            GlobalVars.Instance.thisExperiment = GlobalVars.ExperimentPhase.finish;
            EventManager.Instance.TriggerEvent(new NewCanvasEvent());
        }
        // -------------------------- Stroke Type Selector --------------------------
        if (Input.GetKeyDown(KeyCode.F))
        {
            setStrokeShape("ribbon");
            Debug.Log("FLAT/RIBBON STROKE (F) has been selected.");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            setStrokeShape("cylinder");
            Debug.Log("CYLINDER/ROUND STROKE (C) has been selected.");
        }
        // -------------------------- Stroke Size Selector --------------------------
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetStrokeSize(0.01f);
            Debug.Log("STROKE SIZE: 1 (SMALL) has been chosen with size 0.01");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetStrokeSize(0.02f);
            Debug.Log("STROKE SIZE: 2 (MEDIUM) has been chosen with size 0.02");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetStrokeSize(0.03f);
            Debug.Log("STROKE SIZE: 2 (LARGE) has been chosen with size 0.0.");
        }
        // -------------------------- Translate -------------------------------------
        if (Input.GetKeyDown(KeyCode.T))
        {
            // if switching to translation, get the brush's position, otherwise set it to infinity
            offsetTranslating = !toggleTranslating ? brush.transform.transform.position : Vector3.positiveInfinity;
            toggleTranslating = !toggleTranslating;                         // toggle the translation bool
        }
        // -------------------------- Rotate ----------------------------------------
        if (Input.GetKeyDown(KeyCode.R))
        {
            // if switching to rotation, get the brush's rotation, otherwise set it to infinity
            offsetRotating = !toggleRotating ? brush.transform.transform.rotation.eulerAngles : Vector3.positiveInfinity;
            toggleRotating = !toggleRotating;
        }
        // -------------------------- Undo/Redo -------------------------------------
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Undo();
            Debug.Log("Undo");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Redo();
            Debug.Log("Redo");
        }
        // -------------------------- Delete Stroke ---------------------------------
        if (Input.GetKeyDown(KeyCode.X))
        {
            //ds.GetComponent<DeleteStroke>().deleteStroke(); //(Tanmay) My method, just disables the gameobject
            EventManagerObj.GetComponent<DrawingCanvas>().deleteStroke();
            Debug.Log("Delete (X) pressed");
        }
        // -------------------------- Select Stroke ---------------------------------
        if (Input.GetKeyDown(KeyCode.S))
        {
            //EventManagerObj.GetComponent<DrawingCanvas>().selectStroke();
            //Debug.Log("Select (S) pressed");
            Debug.Log("**************************");
            Debug.Log("Select queue:");
            foreach (int a in selectQueue)
                Debug.Log(a);
        }
        */

        if (toggleTranslating)
        {
            if (!n0)                                                        // dummy object referencing first stroke
            {   // assign n0 to point to first stroke
                n0 = GameObject.Find("StrokeContainer").gameObject.transform.GetChild(0).gameObject;
            }

            Vector3 newPosition = brush.transform.position;                 // get the current position of the brush
            n0.transform.position += (newPosition - offsetTranslating);     // offset the stroke by the calculated difference
            offsetTranslating = newPosition;                                // update the position to the current (prevents continuous translation)
            //n0.transform.position = brush.transform.position;
        }

        if (toggleRotating)
        {   
            if (!n0)                                                        // dummy object referencing first stroke
            {   // assign n0 to point to first stroke
                n0 = GameObject.Find("StrokeContainer").gameObject.transform.GetChild(0).gameObject;
            }

            Vector3 newRotation = brush.transform.rotation.eulerAngles;     // get the current angle of the brush
            Vector3 diff = newRotation - offsetRotating;                    // get the difference from the previously saved rotation
            offsetRotating = newRotation;                                   // update the rotation to the current (prevents continuous rotation)

            // use hack: extract (x,y,z) from stroke name
            string[] parser = n0.name.Split("_");

            // create a pivot from the extract direction
            Vector3 pivotPoint = new Vector3(float.Parse(parser[1]), float.Parse(parser[2]), float.Parse(parser[3]));

            // RotateAround can only do one direction at a time, therefore, update each direction
            // individually with the difference obtained at the beginning of this if-statement
            n0.transform.RotateAround(pivotPoint, Vector3.right, diff.x);
            n0.transform.RotateAround(pivotPoint, Vector3.up, diff.y);
            n0.transform.RotateAround(pivotPoint, Vector3.forward, diff.z);
        }             
    }
    public void menuButtonPressed()
    {
        Debug.Log("menuButtonPressed");
        MenuCanvasObj.GetComponent<MenuCanvas>().toggleMenu();
        //if (MenuCanvasObj.GetComponent<Canvas>().enabled)
        //{
        //    CameraRigObj.GetComponent<LaserPointerEvents>().insideMenuCanvas = true; //if menu is on then set insideMenuCanvas true, for when menu is loaded with cursor pointing at it
        //}
        //else if (!MenuCanvasObj.GetComponent<Canvas>().enabled)
        //{
        //    CameraRigObj.GetComponent<LaserPointerEvents>().insideMenuCanvas = false; //if menu is off then set insideMenuCanvas false, for when menu is unloaded with cursor pointing at it
        //}
    }
    public void triggerPressed() //need to implement modes for trigger properly, maybe access inputs through steamvr library 
    {
        //if (CameraRigObj.GetComponent<LaserPointerEvents>().insideMenuCanvas == false //enable drawing stroke when NOT pointing at menu
        //    && MenuCanvasObj.GetComponent<MenuCanvas>().selectButtonState==false) //enable drawing stroke when NOT in select mode
        if (MenuCanvasObj.GetComponent<MenuCanvas>().selectButtonState==false) //enable drawing stroke when NOT in select mode
        {
            tempState = true;
        }
        if(MenuCanvasObj.GetComponent<MenuCanvas>().selectButtonState == true && MenuCanvasObj.GetComponent<MenuCanvas>().selectModeButtonState==false) //select
        {
            addStrokeSelectQueue(EventManagerObj.GetComponent<DrawingCanvas>().selectStroke());
        }
        if(MenuCanvasObj.GetComponent<MenuCanvas>().selectButtonState == true && MenuCanvasObj.GetComponent<MenuCanvas>().selectModeButtonState == true) //deselect
        {
            removeStrokeSelectQueue(EventManagerObj.GetComponent<DrawingCanvas>().selectStroke());
        }
    }
    public void addStrokeSelectQueue(int nearObject)
    {
        if (nearObject == -1)
            Debug.Log("STROKE NOT FOUND");
        else if(selectQueue.IndexOf(nearObject) == -1)//add only if not already in queue
        {
            selectQueue.Add(nearObject);
            Debug.Log("Added stroke " + EventManagerObj.GetComponent<DrawingCanvas>().AllStrokes[nearObject].stroke.name);

            //Outline stroke
            var outline = EventManagerObj.GetComponent<DrawingCanvas>().AllStrokes[nearObject].stroke.GetComponent<Outline>();
            if (outline) //if outline component already exists
            {
                outline.enabled = true;
            }
            else //if outline component doesnt exist
            {
                outline = EventManagerObj.GetComponent<DrawingCanvas>().AllStrokes[nearObject].stroke.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 10f;
            }
            
        }
    }
    public void removeStrokeSelectQueue(int nearObject)
    {
        if (nearObject == -1)
            Debug.Log("STROKE NOT FOUND");
        if (selectQueue.IndexOf(nearObject) > -1)//remove only if in queue
        {
            selectQueue.Remove(nearObject);
            Debug.Log("Removed stroke " + EventManagerObj.GetComponent<DrawingCanvas>().AllStrokes[nearObject].stroke.name);

            //disable outline
            EventManagerObj.GetComponent<DrawingCanvas>().AllStrokes[nearObject].stroke.GetComponent<Outline>().enabled = false;
        }
    }
    public void clearSelectQueue()
    {
        foreach (int a in selectQueue) //disable outline on all strokes
        {
            EventManagerObj.GetComponent<DrawingCanvas>().AllStrokes[a].stroke.GetComponent<Outline>().enabled = false;
        }
        selectQueue.Clear();
    }
    public void setStrokeShape(string shape)
    {
        GlobalVars.Instance.currentStrokeShape = shape;
    }
    public void triggerReleased()
    {
        //if (CameraRigObj.GetComponent<LaserPointerEvents>().insideMenuCanvas == false) //disable drawing stroke when pointing at menu
        //{
        //    tempState = false;
        //}
        tempState = false;
    }
    public void OnTriggerClick(bool value)
    {
        if (value)
        {
            _drawing = true;
            EventManager.Instance.QueueEvent(new StartDrawingEvent(brush.transform));
        }

        else
        {
            _drawing = false;
            EventManager.Instance.QueueEvent(new EndDrawingEvent(brush.transform));
        }
    }
    public void SetStrokeSize(float size)
    {
        GlobalVars.Instance.currentStrokeSize = size;
    }
    public void Undo() {
        List<GameObject> undoList = GlobalVars.Instance.undoList;

        if(undoList.Any()) {
            // Debug.Log(undoList.Last().name);
            GlobalVars.Instance.redoList.Add(undoList.Last());
            undoList.Last().SetActive(false);
            undoList.Remove(undoList.Last());
        }
        GlobalVars.Instance.consecUndoCount++;
    }
    public void Redo() {
        List<GameObject> redoList = GlobalVars.Instance.redoList;

        if(GlobalVars.Instance.redoList.Count >= 6) {
            GlobalVars.Instance.redoList.RemoveAt(0);
        }
        
        if(redoList.Any()) {
            // Debug.Log(redoList.Last().name);
            GlobalVars.Instance.undoList.Add(redoList.Last());
            redoList.Last().SetActive(true);
            redoList.Remove(redoList.Last());
        }
    }
}
