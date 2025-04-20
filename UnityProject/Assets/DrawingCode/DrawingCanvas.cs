/* 
 * mayra barrera, 2017
 * class that reprenset a canvas (main class that receive event's)
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class DrawingCanvas : MonoBehaviour
{
    //control variables that specify user actions
    private bool drawing;
    private bool controllerVisible;
    private bool strokeVisible;

    //strokes
    private Dictionary<int, Stroke> allStrokes;
    public Dictionary<int, Stroke> AllStrokes   // property
    {
        get { return allStrokes; }   // get method
        set { allStrokes = value; }  // set method
    }
    //private bool firstStroke;
    private int currentStrokeName;
    private int strokeCount;
    public int StrokeCount
    {
        get { return strokeCount; } // Getter
        set { strokeCount = value; } // Setter
    }

    //guide
    private Vector3 startPosition;

    //classes that control user actions
    private StrokeCreationModule strokeCreation;
    private ExperimentControl experimentControl;
    private VisualGuidesControl visualGuidesControl;
    private GazeControll gazeControl;
    private WidgetRenderer widgetRenderer;

    public UndoRedo undoRedo;

    #region initi

    public void Awake()
    {
        //event listeners, almost all of them come from the controllers
        EventManager.Instance.AddListener<StartDrawingEvent>(StartDrawing);
        EventManager.Instance.AddListener<EndDrawingEvent>(EndDrawing);
        EventManager.Instance.AddListener<OnDrawingEvent>(UpdatePosition);

        EventManager.Instance.AddListener<NewCanvasEvent>(DeleteEverything);
        EventManager.Instance.AddListener<DeleteEvent>(DeleteBrush);


        //EventManager.Instance.AddListener<StartRotationEvent>(StartRotate);
        //EventManager.Instance.AddListener<EndRotationEvent>(EndRotate);
        //EventManager.Instance.AddListener<SetLeftControllerGameobjectEvent>(SetLeftController);
    }

    void Start()
    {
        InitiateGlobalVariables();

        GameObject.Find("Stroke Size Panel").GetComponentInChildren<TextMeshProUGUI>().text = "Stroke Size: " + (GlobalVars.Instance.currentStrokeSize).ToString("0.00") + "f";

        allStrokes = new Dictionary<int, Stroke>();
        strokeCount = 0;

        strokeCreation = transform.GetComponent<StrokeCreationModule>();
        experimentControl = transform.GetComponent<ExperimentControl>();
        visualGuidesControl = transform.GetComponent<VisualGuidesControl>();
        gazeControl = transform.GetComponent<GazeControll>();
        widgetRenderer = transform.GetComponent<WidgetRenderer>();
    }

    /* ****************************************************************************************************************
        Global variables for width/diameter, color, eye tracking, and smart guide.
            GlobalVars.Instance.currentStrokeColor: sets the color of the stroke (see 
                https://docs.unity3d.com/ScriptReference/Color.html)
            GlobalVars.Instance.currentStrokeSize: a float that sets the size of the stroke
            GlobalVars.Instance.thisVisualGuide: sets the guide (see GlobalVars.cs enum VisualGuide in lines 124 - 129)
        Raytracing is accomplish in UpdatePosition() line 101.  Commenting the line removes the raytracing.
       **************************************************************************************************************** */
    private void InitiateGlobalVariables()
    {
        drawing = false;                                        // is a stroke currently in progress
        //firstStroke = true;                                     // marks it as the first stroke, covering the hollow structure

        GlobalVars.Instance.currentStrokeColor = Color.blue;    // sets the color of the stroke
        GameObject.Find("MenuCanvas").GetComponent<MenuCanvas>().previewStrokeColor = GlobalVars.Instance.currentStrokeColor; // initializing color preview in menu canvas
        GlobalVars.Instance.currentStrokeSize = 0.01f;          // sets the size of the stroke

        GlobalVars.Instance.currentStrokeDelete = false;        // references not found on this -- removable?
        GlobalVars.Instance.currentStrokeOutlineSize = 0.05f;   // references not found on this -- removable?

        GlobalVars.Instance.useEyeTracking = false;             // references not found on this -- removable?
        GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.none;      // select from: none, SmartGuides, Templates
    }

    #endregion

    #region drawing
    void UpdatePosition(OnDrawingEvent e)
    {
        //current control position
        Transform control = e.transform;
        //EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(100, control.position, Color.green, control.forward, 100));

        // RAYTRACING: comment the line below to disable to raytracing.       
        // EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(100, control.position, Color.green, control.forward, 100));

        //EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(100, control.position, Color.green, control.forward, 100));


        //Update Gaze Position
        // EYETRACKING: leaving this as null, as it removes the magenta sphere from the center of the screen, representing 
        //              the eye tracking location (original setting left as a comment)
        //Transform eyeGaze = null;                               // gazeControl.UpdateGazeInfo();

        //Guide State
        //results: "nothing" for when nothing is on the screen, "controller" for when only the controller, "stroke" for when only stroke, "controllerstrokes" for when both
        #region visible objects
        //string visibleObjects = "nothing";

        if (control.GetComponent<Renderer>().isVisible)
        {
            //inside visual attention
            var headToControllerDirection = control.position - gazeControl.getStartGaze();
            float angle = Vector3.Angle(headToControllerDirection, gazeControl.getGazeRayDirection());
            
            if (angle < 55)//need to make this value global!!
            {
                //visibleObjects = "controller";
            }
            
        }

        /*ArrayList visibleStrokes = ObjectOnCamera();

        if(visibleStrokes.Count > 0)
        {
            if (visibleObjects.Equals("nothing"))
            {
                visibleObjects = "strokes";
            }
            else
            {
                visibleObjects = visibleObjects + "strokes";
            }
        }*/
        #endregion

        //STROKE
        #region Create Stroke
        if (drawing)
        {
            strokeCreation.onDrawing(control);
        }
        #endregion

        /*//VISUAL GUIDES
        #region visual Guides

 
        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.SmartGuides)
        {
            Debug.DrawRay(control.position, control.forward);
            //visualGuidesControl.RemoveSmartGuides();

            if (drawing)
            {
                visualGuidesControl.DrawingGuide(startPosition, currentStrokeName, eyeGaze);
            }
            else
            {
                //Debug.Log(visibleObjects);
                switch (visibleObjects)
                {
                    case "controller":
                        visualGuidesControl.ControllerGuide(control, eyeGaze);
                        break;
                    case "strokes":
                        visualGuidesControl.StrokeGuide(visibleStrokes, eyeGaze);
                        break;
                    case "controllerstrokes":
                        visualGuidesControl.EyeGazeControllerGuide(visibleStrokes, control, eyeGaze);
                        break;
                    default:
                        visualGuidesControl.EyeGazeGuide(eyeGaze);
                        break;
                }
            }
        }
        #endregion
        */

        //ROTATE
        #region RotateStrokes
        /*if (!drawing)
        {
            switch (GlobalVars.Instance.thisRotationType)
            {
                case GlobalVars.RotationType.twoHand:
                    rotationControl.TwoHandRotateObject(control);
                    break;
                case GlobalVars.RotationType.oneHand:
                    rotationControl.OneHandRotateObject();
                    break;
                default:
                    //Debug.Log("walk");
                    break;
            }
        }*/
        #endregion
    }

    void StartDrawing(StartDrawingEvent e)
    {
        undoRedo.canUndoRedo = false;

        //Debug.Log("Start Draw");
        //standard control variables
        drawing = true;

        //Debug.Log(visualGuidesControl.showGuidesType);

        //Visual Guides
        visualGuidesControl.showGuidesType = 2;

        //Log new stroke start (position object)
        //Debug.Log(e.brush.transform.position);
        //experimentControl.StartStroke(e.brush.transform.position);

        //Create new stroke
        if (allStrokes.ContainsKey(strokeCount))
        {
            //sometimes I have an error about duplicated key, this is to avoid that error (NOT idea why the error happens)
            strokeCount++;
        }
       
        allStrokes.Add(strokeCount, new Stroke(strokeCount, GlobalVars.Instance.currentStrokeSize, GlobalVars.Instance.currentStrokeColor));
        strokeCreation.StartDrawing(allStrokes[strokeCount], e.brush.position);

        currentStrokeName = strokeCount;
        startPosition = e.brush.position;
    }

    void EndDrawing(EndDrawingEvent e)
    {
        drawing = false;
        Transform control = e.brush.transform;

        //Visual Guides
        visualGuidesControl.showGuidesType = 1;
        visualGuidesControl.RemoveGlow();

        //Log new stroke end (position object)
        //experimentControl.EndStroke(control.position);

        //Close new stroke      
        bool exist = strokeCreation.EndDrawing(control);

        if (!exist)
        {
            //stroke
            Destroy(allStrokes[strokeCount].stroke);
            allStrokes[strokeCount].DestroyStroke();
            allStrokes.Remove(strokeCount);
            strokeCount--;
        }
        else
        {
            undoRedo.NewStrokeAction(strokeCount); //undoredo
        }

        strokeCount++;
        currentStrokeName = -1;
        startPosition = Vector3.zero;

        undoRedo.canUndoRedo = true;
    }
    #endregion


    #region menuActions
 
    //clean whole canvas
    private void DeleteEverything(NewCanvasEvent e)
    {
        //control log
        experimentControl.Phase(strokeCount);

        //visual guides
        visualGuidesControl.showGuidesType = (GlobalVars.Instance.thisExperiment == GlobalVars.ExperimentPhase.drawing) ? 1 : 0;
        visualGuidesControl.RemoveSmartGuides();

        //strokes
        foreach (Stroke s in allStrokes.Values)
        {
            Destroy(s.stroke);
        }

        allStrokes.Clear();
        strokeCount = 0;

        //global
        InitiateGlobalVariables();
    }

    private void DeleteBrush(DeleteEvent e)
    {
        //find nearby
        Vector3[] temp = FindNearSurface(e.position, e.right, e.up);
        int nearObject = (int)temp[0].x;

        //remove gameobject
        Debug.Log(nearObject);
        if (allStrokes.ContainsKey(nearObject))
        {
            Destroy(allStrokes[nearObject].stroke);
            allStrokes[nearObject].DestroyStroke();
            allStrokes.Remove(nearObject);
        }
    }


    #endregion

    #region FindObjects
    private ArrayList ObjectOnCamera()
    {
        ArrayList list = new ArrayList();
 
        //check if strokes on Camera
        if (allStrokes.Count > 0)
        {
            foreach (KeyValuePair<int, Stroke> tempStroke in allStrokes)
            {
                Stroke currentStroke = tempStroke.Value;
                
                if (currentStroke.stroke.GetComponent<Renderer>().isVisible)
                {
                    list.Add(currentStroke);
                }
            }
        }

        return list;
    }

    private Vector3[] FindNearSurface(Vector3 position, Vector3 controlRight, Vector3 controlForward)
    {
        int strokeName = -1;
        float angularDistClosestToVertex = Mathf.Infinity;

        Vector3 closestVertexPosition = Vector3.zero;

        //check if there are strokes
        if (allStrokes.Count > 0)
        {
            //find the closest stroke to me
            foreach (KeyValuePair<int, Stroke> tempStroke in allStrokes)
            {
                if (tempStroke.Key != currentStrokeName)
                {
                    float toAngularVertex = Mathf.Infinity;
                    Vector3 Vertex = Vector3.zero;

                    //for each stroke calculate the distance to each vertex
                    for (int i = 0; i < tempStroke.Value.LastIndex(); i++)
                    {
                        //angular direction from controller to vertex
                        Vector3 directionToVertex = VectorMathsLibrary.DirectionBetweenPoints(position, tempStroke.Value.GetStrokePosition(i).position);
                        float angularDirection = 1.01f - Mathf.Abs(Vector3.Dot(controlForward, directionToVertex));

                        float angularDistance = VectorMathsLibrary.DistanceBetweenTwoPoints(position, tempStroke.Value.GetStrokePosition(i).position) * angularDirection;

                        if (angularDistance < toAngularVertex)
                        {
                            toAngularVertex = angularDistance;
                            Vertex = tempStroke.Value.GetStrokePosition(i).position;
                        }
                    }

                    //if closest save values
                    if (toAngularVertex < angularDistClosestToVertex)
                    {
                        strokeName = tempStroke.Key;
                        closestVertexPosition = Vertex;
                        angularDistClosestToVertex = toAngularVertex;
                    }
                }
            }
        }

        Vector3[] results = new Vector3[2];
        results[0] = new Vector3(strokeName, angularDistClosestToVertex, VectorMathsLibrary.DistanceBetweenTwoPoints(position, closestVertexPosition));
        results[1] = closestVertexPosition;

        return results;
    }
    #endregion
    public GameObject RightController;
    public int selectStroke()
    {
        //Transform ControllerRightTransform = GameObject.Find("Controller (right)").transform; ///steamvr
        Transform ControllerRightTransform = RightController.transform;
        Vector3[] results = FindNearSurface(ControllerRightTransform.position, ControllerRightTransform.right, ControllerRightTransform.forward);
        //Debug.Log(results[0]);
        //Debug.Log(results[1]);
        int nearObject = (int)results[0].x;
        if (allStrokes.ContainsKey(nearObject))
        {
            //Debug.Log(allStrokes[nearObject].stroke.name);
            return nearObject;
        }
        return -1;
    }
    public void deleteStroke() {
        int nearObject = selectStroke();
        if (nearObject == -1)
            Debug.Log("STROKE DOES NOT EXIST AND CANNOT BE DELETED");
        else
        {
            Debug.Log("Deleting " + allStrokes[nearObject].stroke.name);
            Destroy(allStrokes[nearObject].stroke);
            allStrokes[nearObject].DestroyStroke();
            allStrokes.Remove(nearObject);
        }
    }
    public void changeStrokeColor(Color rgb)
    {
        GlobalVars.Instance.currentStrokeColor = rgb;
    }
    public Color retCurrentStrokeColor()
    {
        //Debug.Log(GlobalVars.Instance.currentStrokeColor);
        return GlobalVars.Instance.currentStrokeColor;
    }
}
