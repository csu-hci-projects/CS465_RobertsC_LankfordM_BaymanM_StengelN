using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Valve.VR;
//using Tobii.G2OM;
//using Tobii.XR.Internal;
//using Tobii.XR;
public class VisualGuidesControl : MonoBehaviour
{

    //variables that I'm planning to delete (I'm == Mayra)
    GameObject userHead;
    public Dictionary<int, Stroke> allStrokes;
    public GameObject Renderer;
    //public GameObject Controller;
    public int verticesLookingRange = 15;

    public int _showGuidesType; //0-nothing, 1-planning, 2-drawing
    //private List<RibbonVertex> _myPositions;

    //private bool hit;
    private float startStroke_Length;//length of closest stroke before user start drawing
    private Vector3 startDirection; //start direction from controller to vertex
    private Vector3 startStroke_snappedVertex;//vertex position of closest stroke before user start drawing

    private Vector3 gazeStartPos;
    private Vector3 gazeRayDirection;

    //private StrokeCreationModule strokeCreation;
    private Stroke strokepos;

    #region smartGuides Variables 

    //eye gaze info
    int VISUALATTENTIONAREA = 55; // in degrees around gaze

    //crosshair
    private Vector3[] fixedController;

    //glow
    private Vector3[] future_snappedVertex;//future vertex position based on start and controller

    //Grid
    private float _gridStep;
    private float _gridLength;
    private int _gridLines;

    private Vector3[] gridStartPositions; // x, y
    #endregion

    void Start()
    {
        userHead = GameObject.Find("Camera");
        _showGuidesType = 2;

        #region set smartGuides

        //hit = false;
        startStroke_Length = 0;
        startStroke_snappedVertex = Vector3.zero;
        startDirection = Vector3.zero;

        //crosshair
        fixedController = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };

        //glow
        future_snappedVertex = new Vector3[2] { Vector3.zero, Vector3.zero };

        //grid / lines
        _gridStep = (GlobalVars.gridLength / GlobalVars.gridLines);
        _gridLines = GlobalVars.gridLines;
        _gridLength = GlobalVars.gridLength;

        gridStartPositions = new Vector3[(_gridLines + 1) * (_gridLines + 1) * (_gridLines + 1)]; //number of lines * each axis       
        #endregion

        #region createGrid
        //select back left corner of VR space
        Vector3 startPoint = GeometricDetectionLibrary.PointLine(Vector3.zero, -Vector3.right, _gridLength / 2);
        startPoint = GeometricDetectionLibrary.PointLine(startPoint, Vector3.forward, _gridLength / 2);

        float currentStepX = 0;
        float currentStepY = 0;
        float currentStepZ = 0;

        for (int z = 0, i = 0; z < _gridLines; z++)
        {
            currentStepY = 0;

            for (int y = 0; y < _gridLines; y++)
            {
                currentStepX = 0;

                for (int x = 0; x < _gridLines; x++, i++)
                {

                    float positionX = GeometricDetectionLibrary.VectorValue(startPoint, Vector3.right, currentStepX, 1);
                    float positionY = GeometricDetectionLibrary.VectorValue(startPoint, Vector3.up, currentStepY, 2);
                    float positionZ = GeometricDetectionLibrary.VectorValue(startPoint, -Vector3.forward, currentStepZ, 3);

                    gridStartPositions[i] = new Vector3(positionX, positionY, positionZ);

                    currentStepX += _gridStep;

                    //Debug.Log(gridStartPositions[i]);
                }

                currentStepY += _gridStep;
            }

            currentStepZ += _gridStep;
        }

        //test grid
        /*
        for (int i = 0, j = 0; i < gridStartPositions.Length; i++, j+=3)
        {
            EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(i, gridStartPositions[i], Color.blue, 0.05f));

            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(j, gridStartPositions [i], Color.red, Vector3.right, _gridStep));
            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(j+1, gridStartPositions[i], Color.green, Vector3.up, _gridStep));
            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(j+2, gridStartPositions[i], Color.blue, -Vector3.forward, _gridStep));
        }
        */
        #endregion
    }

    #region EyeTrackingMethods
    //case a: if drawing = Show grid start drawing position, extend with eyes
    public void DrawingGuide(Vector3 startStroke_snappedVertex, int currentStrokeName, Transform eyeGaze)
    {
        ShowLinesExtension(startStroke_snappedVertex, eyeGaze.position);
    }

    //case b: if stroke&controller = Show grid around stroke vertice nearest to controller, extend eye gaze
    public void EyeGazeControllerGuide(ArrayList visibleStrokes, Transform controller, Transform eyeGaze)
    {
        //closet vertice to controller
        Vector3 verticePosition = ClosetVertice_EuclideanDistance(controller.position, visibleStrokes);

        Debug.Log("Vertice Position : " + verticePosition);

        //grid on stroke vertice, extend eye gaze
        ShowLinesExtension(verticePosition, eyeGaze.position);
    }

    //case c: if controller = Show grid around controller, extend eye gaze
    public void ControllerGuide(Transform control, Transform eyeGaze)
    {
        ShowLinesExtension(control.position, eyeGaze.position);
    }

    //case d: if stroke = Show grid around stroke, grid moves
    public void StrokeGuide(ArrayList visibleStrokes, Transform eyeGaze)
    {
        //closet vertice to eye gaze
        Vector3 verticePosition = ClosetVertice_EuclideanDistance(eyeGaze.position, visibleStrokes);

        //create new vector with stroke depth
        Vector3 mixedPosition = new Vector3(eyeGaze.position.x, eyeGaze.position.y, verticePosition.z);

        //grid on stroke vertice, extend eye gaze
        ShowLines(mixedPosition);
    }

    //case e: if nothing - show grid around eye gaze position
    public void EyeGazeGuide(Transform eyeGaze)
    {
        ShowLines(eyeGaze.position);
    }

    //angular distance (considers visual attention area)
    private Vector3 ClosetsVertice_AngularDistance(Vector3 position, Vector3 right, Vector3 forward, ArrayList strokes)
    {

        float angularDistClosestToVertex = Mathf.Infinity;
        Vector3 closestVertexPosition = Vector3.zero;

        foreach (Stroke stroke in strokes)
        {

            float toAngularVertex = Mathf.Infinity;
            Vector3 Vertex = Vector3.zero;

            //for each stroke calculate the distance to each vertex
            for (int i = 0; i < stroke.LastIndex(); i++)
            {

                Vector3 vertexPosition = stroke.GetStrokePosition(i).position;
                Vector3 headToItemRay = vertexPosition - gazeStartPos; // get direction of ray from head to item position, by substracting the item position from the camera/head position
                                                                       // get visual angle of head-target ray to gaze ray
                float currentItemAngle = Vector3.Angle(gazeRayDirection, headToItemRay); // an angle value 0-360

                if (currentItemAngle < VISUALATTENTIONAREA) //vertex inside visual attention
                {
                    //angular direction from controller to vertex
                    Vector3 directionToVertex = VectorMathsLibrary.DirectionBetweenPoints(position, stroke.GetStrokePosition(i).position);
                    float angularDirection = 1.01f - Mathf.Abs(Vector3.Dot(forward, directionToVertex));

                    float angularDistance = VectorMathsLibrary.DistanceBetweenTwoPoints(position, stroke.GetStrokePosition(i).position) * angularDirection;

                    if (angularDistance < toAngularVertex)
                    {
                        toAngularVertex = angularDistance;
                        Vertex = stroke.GetStrokePosition(i).position;
                    }
                }
            }

            //if closest save values
            if (toAngularVertex < angularDistClosestToVertex)
            {
                closestVertexPosition = Vertex;
                angularDistClosestToVertex = toAngularVertex;
            }
        }

        return closestVertexPosition;
    }

    //euclidean distance (considers visual attention area)
    private Vector3 ClosetVertice_EuclideanDistance(Vector3 position, ArrayList strokes)
    {

        float minDistance = Mathf.Infinity;
        Vector3 closestVertex = Vector3.zero;


        EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(567, position, Color.red, 0.02f));

        foreach (Stroke stroke in strokes)
        {
            for (int i = 0; i < stroke.LastIndex(); i++)
            {

                Vector3 vertexPosition = stroke.GetStrokePosition(i).position;

                int widgetName = -1 * i;
               
                Vector3 headToItemRay = vertexPosition - gazeStartPos; // get direction of ray from head to item position, by substracting the item position from the camera/head position
                                                                       // get visual angle of head-target ray to gaze ray
                float currentItemAngle = Vector3.Angle(gazeRayDirection, headToItemRay); // an angle value 0-360

                if (currentItemAngle < VISUALATTENTIONAREA) //vertex inside visual attention
                {
                    float distance = Vector3.Distance(vertexPosition, position);

                    Debug.Log(widgetName + "  " + distance);

                    if (distance < minDistance) //closest vertex to eye gaze
                    {
                        
                        closestVertex = vertexPosition;
                        minDistance = distance;
                    }
                }
            }
        }

        EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(566, closestVertex, Color.magenta, 0.04f));

        return closestVertex;
    }

    //TODO!!
    private void ShowLinesExtension(Vector3 centerPosition, Vector3 eyePosition)
    {
        ShowLinesWithExtension(centerPosition, eyePosition);
        
    }

    //This should be working
    private void ShowLines(Vector3 controlPosition)
    {
        EventManager.Instance.QueueEvent(new DeleteAllWidgetsEvent());

        List<GridPoints> nearbyGridPoints = FindNearGridPos(controlPosition);

        int countPoint = 0;
        foreach (GridPoints point in nearbyGridPoints)
        {
            float distance = VectorMathsLibrary.DistanceBetweenTwoPoints(point.position, userHead.transform.position);

            if (distance > GlobalVars.safeAreaRadius)
            {
                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint, point.position, Color.black, Vector3.right, _gridStep));
                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 1, point.position, Color.black, -Vector3.right, _gridStep));

                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 4, point.position, Color.black, Vector3.forward, _gridStep));
                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 5, point.position, Color.black, -Vector3.forward, _gridStep));
            }

            else
            {
                int whichPoints = VectorMathsLibrary.SignedRelationshipBetweenTwoPlanes(userHead.transform.forward, Vector3.right);

                int sign = (whichPoints < 0) ? -1 : 1;
                GlobalVars.geometricRelation relationship = (GlobalVars.geometricRelation)Mathf.Abs(whichPoints);

                //Debug.Log("entre " + relationship + "  " + sign);

                if (relationship == GlobalVars.geometricRelation.parallelism)
                {
                    Debug.Log("entre");
                    if (sign < 0)
                    {
                        Debug.Log("entre 2");
                        EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint, point.position, Color.red, Vector3.right, _gridStep));
                        EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 1, point.position, Color.yellow, -Vector3.right, _gridStep));
                    }
                    else
                    {
                        EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint, point.position, Color.yellow, -Vector3.right, _gridStep));
                    }
                }
            }

            //always up
            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 2, point.position, Color.black, Vector3.up, _gridStep));
            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 3, point.position, Color.black, -Vector3.up, _gridStep));

            countPoint = countPoint + 6;
        }
    }

    #endregion


    private void ShowLinesWithExtension(Vector3 controlPosition, Vector3 extensionPosition)
    {
        //EventManager.Instance.QueueEvent(new DeleteAllWidgetsEvent());

        ShowLines(controlPosition);
        //Debug.Log("Show Linex Extension");

        List<GridPoints> nearbyGridPointsExtension = FindNearGridPos(extensionPosition);

        int countPoint = 200;
        foreach (GridPoints point in nearbyGridPointsExtension)
        {
            float distance = VectorMathsLibrary.DistanceBetweenTwoPoints(point.position, userHead.transform.position);

            if (distance > GlobalVars.safeAreaRadius)
            {
                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint, point.position, Color.black, Vector3.right, _gridStep));
                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 1, point.position, Color.black, -Vector3.right, _gridStep));

                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 4, point.position, Color.black, Vector3.forward, _gridStep));
                EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 5, point.position, Color.black, -Vector3.forward, _gridStep));
            }

            else
            {
                int whichPoints = VectorMathsLibrary.SignedRelationshipBetweenTwoPlanes(userHead.transform.forward, Vector3.right);

                int sign = (whichPoints < 0) ? -1 : 1;
                GlobalVars.geometricRelation relationship = (GlobalVars.geometricRelation)Mathf.Abs(whichPoints);

                //Debug.Log("entre " + relationship + "  " + sign);

                if (relationship == GlobalVars.geometricRelation.parallelism)
                {
                    Debug.Log("entre");
                    if (sign < 0)
                    {
                        Debug.Log("entre 2");
                        EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint, point.position, Color.red, Vector3.right, _gridStep));
                        EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 1, point.position, Color.yellow, -Vector3.right, _gridStep));
                    }
                    else
                    {
                        EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint, point.position, Color.yellow, -Vector3.right, _gridStep));
                    }
                }
            }

            //always up
            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 2, point.position, Color.black, Vector3.up, _gridStep));
            EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(countPoint + 3, point.position, Color.black, -Vector3.up, _gridStep));

            countPoint = countPoint + 6;
        }

    }
    public void RemoveSmartGuides()
    {
        EventManager.Instance.QueueEvent(new DeleteAllWidgetsEvent());
    }

    public void RemoveGlow()
    {
        EventManager.Instance.QueueEvent(new DeleteAllWidgetsEvent());
    }
     
    //all of these methods (inside Smartguides) will be deleted in the future, I'm just grabing the code that we will need
    #region smartGuides methods

    //I'm leaving these as they might help with the extension method (I'm == Mayra)
    private void ShowGlow(Vector3 controlRight, Vector3 snapping_VertexPosition, float snapping_distance, float strokeLength)
    {
        Color glowColor = (snapping_distance<0.05) ? Color.green : Color.Lerp(Color.yellow, Color.red, NormalizeDistance(snapping_distance));
        EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(1, snapping_VertexPosition, glowColor, 0.07f));

        if(snapping_distance < 0.05)
        {
            future_snappedVertex[0] = GeometricDetectionLibrary.PointLine(snapping_VertexPosition, controlRight, strokeLength);
            EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(2, future_snappedVertex[0], Color.red, 0.07f));

            future_snappedVertex[1] = GeometricDetectionLibrary.PointLine(snapping_VertexPosition, -controlRight, strokeLength);
            EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(3, future_snappedVertex[1], Color.red, 0.07f));

            //hit = true;
        }

        else
        {
            EventManager.Instance.QueueEvent(new DeleteVisualWidgetEvent(2));
            EventManager.Instance.QueueEvent(new DeleteVisualWidgetEvent(3));

            //hit = false;
        }
    }

    private void ShowGlowDrawing(Vector3 controlPosition, Vector3 controlRight)
    {

        //see which direction, delate other point
        float distanceRight = VectorMathsLibrary.DistanceBetweenTwoPoints(controlPosition, future_snappedVertex[0]);
        float distanceLeft = VectorMathsLibrary.DistanceBetweenTwoPoints(controlPosition, future_snappedVertex[1]);

        float current_distance = Mathf.Infinity;
        Vector3 futureVertex = Vector3.zero;

        if (distanceRight > distanceLeft)
        {
            current_distance = distanceLeft;
            futureVertex = future_snappedVertex[1];

            EventManager.Instance.QueueEvent(new DeleteVisualWidgetEvent(2));
        }

        else
        {
            current_distance = distanceRight;
            futureVertex = future_snappedVertex[0];

            EventManager.Instance.QueueEvent(new DeleteVisualWidgetEvent(3));
        }

        //hit future snapped
        if (current_distance < 0.05)
        {
                startStroke_snappedVertex = futureVertex;
                EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(1, startStroke_snappedVertex, Color.green, 0.07f));
        }
        else
        {
            Color glowColor = Color.Lerp(Color.yellow, Color.red, NormalizeDistance(current_distance));
            EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(2, futureVertex, glowColor, 0.07f));
        }


        //hit currentStart
        float inside_Sphere = VectorMathsLibrary.DistanceBetweenTwoPoints(controlPosition, startStroke_snappedVertex);

        if(inside_Sphere < 0.05)
        {
            future_snappedVertex[0] = GeometricDetectionLibrary.PointLine(startStroke_snappedVertex, controlRight, startStroke_Length);
            EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(2, future_snappedVertex[0], Color.red, 0.07f));

            future_snappedVertex[1] = GeometricDetectionLibrary.PointLine(startStroke_snappedVertex, -controlRight, startStroke_Length);
            EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(3, future_snappedVertex[1], Color.red, 0.07f));
        }
    }

    private void ShowCrosshair(Transform control, bool drawing, bool controllerVisible, bool strokeVisible)
    {

        /*

        Debug.DrawRay(control.position, -fixedController[0]);

        Ray toTheLeft = new Ray(control.position, -fixedController[0]);

        Debug.DrawRay(gazeStartPos, gazeRayDirection);

        Ray fromGaze = new Ray(gazeStartPos, gazeRayDirection);

        //GameObject pointOnGazeRay = new GameObject();
        //GameObject pointOnControllerGizmo = new GameObject();

        Debug.Log(gazeStartPos + " + " + fromGaze.GetPoint(2f) + " + " + control.position + " + " + toTheLeft.GetPoint(2f));

        Vector3 intersectionPointTop = GetIntersectionPointCoordinatesOnTop(gazeStartPos, fromGaze.GetPoint(2f), control.position, toTheLeft.GetPoint(2f));

        Debug.Log(intersectionPointTop.ToString("F4"));

        Vector3 intersectionPointFront = GetIntersectionPointCoordinatesOnFront(intersectionPointTop, (intersectionPointTop + Vector3.up * 3), control.position, toTheLeft.GetPoint(2f));

        Vector3 intersection = new Vector3(intersectionPointTop.x, intersectionPointFront.y, intersectionPointTop.z);

        Debug.DrawRay(intersection, Vector3.up);
        Ray ray1 = new Ray(gazeStartPos, gazeRayDirection);


        // Vector from controller position to gaze position
        Vector3 headControlRay = control.position - gazeStartPos;
        float angle = Vector3.Angle(gazeRayDirection, headControlRay);

        /*if (angle < 90f)
        {
            ShowLines(intersection);

        }
        else
        {
            ShowLines(ray1.GetPoint(0.5f));

            Debug.Log(angle);
        }*/

        /*
            int VISUALATTENTIONAREA = 55; // in degrees around gaze
            
            //strokeVisible = isStrokeVisible(VISUALATTENTIONAREA);

        Debug.Log("Controller Visible : " + controllerVisible);
        Debug.Log("Stroke Visible : " + strokeVisible);

        if (drawing)
            {
                // grid shows at controller pos.
                ShowLines(control.position);
                //TODO frid extends to gaze position
                // opacity
            }
            else
            {
            controllerVisible = isControllerVisible(control, VISUALATTENTIONAREA);
            if (controllerVisible)
                {
                strokeVisible = isStrokeVisible(VISUALATTENTIONAREA);
                if (strokeVisible)
                {
                    // grid shows at closest stroke to controller
                    //List<GameObject> strokeListInScene = new List<GameObject>();
                    List<GameObject> strokeListInScene = new List<GameObject>();
                    strokeListInScene = getObjectListInScene(VISUALATTENTIONAREA);
                    float minDistance = 10;
                    GameObject closestGameobject = null;
                    foreach (GameObject stroke in strokeListInScene)
                    {
                        if (Vector3.Distance(stroke.transform.position, control.position) < minDistance)
                        {
                            closestGameobject= stroke;
                            minDistance = Vector3.Distance(stroke.transform.position, control.position);
                        }
                    }
                    ShowLines(closestGameobject.transform.position);
                    //opacity



                }
                else
                {
                    //grid shows at controller pos.
                    ShowLines(control.position);

                    //opacity
                }
                }
                else
                {
                strokeVisible = isStrokeVisible(VISUALATTENTIONAREA);
                    if (strokeVisible)
                    {
                        //grid shows at stroke depth
                        var centerpoint = getCenterOfVisibleStrokes(VISUALATTENTIONAREA);
                    //follows eye movement
                    //showGrid(showDepth); // this method finds grid positions at this depth.

                    ShowLines(centerpoint); // TODO test here after finishing the other todos
                    ShowLines(ray1.GetPoint(0.5f)); // TODO test here after finishing the other todos
                    }
                    else
                    {
                    /*
                        if (angle < 70f)
                        {
                            ShowLines(intersection);

                        }
                        else
                        {
                            ShowLines(ray1.GetPoint(0.5f));
                        }
                   

                    ShowLines(ray1.GetPoint(0.5f));
                    //opacity
                }
            }
            }

        



        float dist = Vector3.Distance(gazeStartPos, intersectionPointTop);
        //Debug.DrawRay(gazeStartPos, control.position);
        //print("Distance to other: " + dist);

        */
        
    }


    //why using these?
    public Vector3 GetIntersectionPointCoordinatesOnTop(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2)
    {
        float tmp = (B2.x - B1.x) * (A2.z - A1.z) - (B2.z - B1.z) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.z - A1.z) - (A1.z - B1.z) * (A2.x - A1.x)) / tmp;

        return new Vector3(
            B1.x + (B2.x - B1.x) * mu,
            0,
            B1.z + (B2.z - B1.z) * mu
        );
    }

    public Vector3 GetIntersectionPointCoordinatesOnFront(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        return new Vector3(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu,
            0
        );
    }

    #endregion


    #region Reusable methods

    private List<GridPoints> FindNearGridPos(Vector3 controlPosition)
    {
        List<GridPoints> nearbyGridPoints = new List<GridPoints>();

        float closestDist = Mathf.Infinity;
        float furtherDist = 0;

        for (int i = 0; i < gridStartPositions.Length; i++)
        {
            float currentDist = VectorMathsLibrary.DistanceBetweenTwoPoints(controlPosition, gridStartPositions[i]);

            if (currentDist < GlobalVars.visibleGridRadius)
            {
                GridPoints newPoint = new GridPoints(i,currentDist,gridStartPositions[i]);
                nearbyGridPoints.Add(newPoint);

                if (currentDist < closestDist)
                {
                    closestDist = currentDist;
                }

                if (currentDist > furtherDist)
                {
                    furtherDist = currentDist;
                }
            }
        }

        float maxDistance = furtherDist - closestDist;

        foreach (GridPoints p in nearbyGridPoints)
        {
            float snappedDistance = p.distance / maxDistance; //between 0 - 1

            float opacity = Mathf.Lerp(1, 0.1f, snappedDistance);

            Color newColor = Color.Lerp(Color.green, Color.yellow, snappedDistance);
            p.color = new Color(newColor.r, newColor.g, newColor.b, opacity);
        }

        return nearbyGridPoints;
    }

    

    private float NormalizeDistance(float distance)
    {
        float snappedDistance = 1;

        if (distance < 0.3f)
        {
            snappedDistance = distance / 0.3f;
        }

        return snappedDistance;
    }

    private WorldInfo GetWorldInfo(float angle)
    {
        Quaternion rotation;
        Color planeColor;

        //look at X plane
        if (angle > 0.85)
        {
            rotation = VectorMathsLibrary.RotationFromThreeVectors(Vector3.up, Vector3.forward, Vector3.up, Vector3.right);
            planeColor = Color.red;
        }

        //look at Z plane
        else if (angle <= 0.55)
        {
            rotation = VectorMathsLibrary.RotationFromThreeVectors(Vector3.up, Vector3.right, Vector3.up, Vector3.forward);
            planeColor = Color.blue;
        }

        // look acute to world
        else
        {
            //long way because I'm lazy
            Vector3 posInZ = GeometricDetectionLibrary.PointLine(Vector3.zero, Vector3.forward, 5);
            Vector3 posInX = GeometricDetectionLibrary.PointLine(Vector3.zero, Vector3.right, 5);

            Vector3 midPoint = VectorMathsLibrary.MiddlePoint(posInZ, posInX);

            Vector3 worldAcuteNormal = VectorMathsLibrary.DirectionBetweenPoints(Vector3.zero, midPoint);
            Vector3 worldAcuteRight = Vector3.Cross(worldAcuteNormal, Vector3.up);

            rotation = VectorMathsLibrary.RotationFromThreeVectors(Vector3.up, worldAcuteRight, Vector3.up, worldAcuteNormal);
            planeColor = new Color(0.9f, 0.49f, 0.04f);
        }

        return new WorldInfo(rotation, planeColor);
    }


    #endregion

    #region accessors
    public int showGuidesType
    {
        set { _showGuidesType = value; }
        get { return _showGuidesType; }
    }
    #endregion
}

public class GridPoints
{
    public int index;
    public float distance;
    public Vector3 position;
    public Color   color;

    public GridPoints(int _index, float _distance, Vector3 _position)
    {
        index = _index;
        distance = _distance;
        position = _position;

        color = Color.black;
    }
}

public class WorldInfo
{
    public Quaternion rotation;
    public Color color;

    public WorldInfo(Quaternion _rotation, Color _color)
    {
        rotation = _rotation;
        color = _color;
    }
}
//#endregion