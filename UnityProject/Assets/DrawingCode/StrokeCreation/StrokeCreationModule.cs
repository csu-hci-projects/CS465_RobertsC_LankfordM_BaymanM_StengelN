/* mayra barrera, 2017
 * class to beautify strokes */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeCreationModule : MonoBehaviour
{
    //stroke containers
    

    //currentStroke
    private Stroke _currentStroke;
    private int _currentStrokeVertex;

    //control position
    private Vector3 _startPosition;
    private Vector3 _previousPosition;
    private Vector3 _position;
    private Matrix4x4 directionPose;

    //features
    //private int _movDirCase;
    private bool _newStroke;

    //test
    private int rings = 0;

    private List<Vector3> strokePositions = new List<Vector3>();

    private GameObject currentCreatedStroke;

    void Start()
    {
        _startPosition = Vector3.zero;
        _previousPosition = Vector3.zero;
        _position = Vector3.zero;
        directionPose = Matrix4x4.identity;

        //_movDirCase = 0;

        _newStroke = false;
    }

    public void PopulateUndoList(Stroke currentStroke){
        Stroke newestStroke = currentStroke;
        GameObject newestGameObject;
        GameObject strokeParent = GameObject.Find("StrokeContainer");
        int redoListSize = GlobalVars.Instance.redoList.Count;
        int consecUndoCount = GlobalVars.Instance.consecUndoCount;

        newestGameObject = strokeParent.transform.Find("n" + newestStroke.id).gameObject;
        
        if(GlobalVars.Instance.undoList.Count >= 5) {
            GlobalVars.Instance.undoList.RemoveAt(0);
        }
        
        GlobalVars.Instance.undoList.Add(newestGameObject);
        
        if(GlobalVars.Instance.consecUndoCount > 0) { // Clear the redo list if 
            // GlobalVars.Instance.redoList.RemoveRange(redoListSize - consecUndoCount, redoListSize); 
            GlobalVars.Instance.redoList.Clear();
        }
    }


    #region draw
    public void StartDrawing(Stroke currentStroke, Vector3 startPosition)
    {
        _currentStroke = currentStroke;
        _currentStrokeVertex = 0;
        _startPosition = startPosition;

        NewStrokeGameobject("n" + _currentStroke.id, _currentStroke.ribbonWidth, _currentStroke.ringVertex,  _currentStroke.ribbonColor);
        
        PopulateUndoList(currentStroke);
        
        _newStroke = true;
    }

    

    public void onDrawing(Transform control)
    {
         
        //add position to stroke mesh
        #region first time
        if (_newStroke)
        {

            CreateMesh(_startPosition, control.forward, directionPose);
            _currentStroke.AddStrokePosition(_startPosition, control.forward);

            _newStroke = false;
            _previousPosition = _startPosition;
            strokePositions.Add(control.transform.position);
        }
        #endregion

        #region otherTimes
        else
        {
            //natural unmodified  position
            _position = control.position;

            //to space the points
            float distance = Mathf.Abs(Vector3.Distance(_position, _previousPosition));

            if (distance > GlobalVars.segmentChangeDistance && rings < 2)
            {
                //calculates the direction matrix (only one time per stroke)
                directionPose = Matrix4x4.Rotate(GetControlDirectionMatrix(control));

                CreateMesh(_position, control.forward, directionPose);
                _currentStroke.AddStrokePosition(_position, control.forward);

                _previousPosition = _position;

                //for test
                //rings++;
            }
        }
        #endregion
    }

    public bool EndDrawing(Transform control)
    {

        //see if stroke exist, remove extra sections without enough points
        bool strokeExist = _currentStrokeVertex > 1 ? true : false;

        #region save final values
        if (strokeExist)
        {
            //directionPose = Matrix4x4.Rotate(GetControlDirectionMatrix(control));

            //add final points to the stroke
            EndStrokeMesh(control.position, directionPose);
        }
        #endregion

        //reset variables
        _currentStroke = null;

        _position = Vector3.zero;
        _previousPosition = Vector3.zero;
        _startPosition = Vector3.zero;

        // this is a quick hack storing position info on a gameobject
        Vector3 centroid = Vector3.zero;

        foreach(Vector3 tmp in strokePositions)
        {
            centroid += tmp;
        }
        centroid /= strokePositions.Count;

        currentCreatedStroke.name += "_" + DrawController.rightBrush.transform.position.x + "_" + DrawController.rightBrush.transform.position.y + "_" + DrawController.rightBrush.transform.position.z;
        strokePositions.Clear();
        currentCreatedStroke = null;

        return strokeExist;
    }
    
    public Quaternion GetControlDirectionMatrix(Transform control)
    {
        Vector3 movementDirection_normal = VectorMathsLibrary.DirectionBetweenPoints(_position, _previousPosition);

        GlobalVars.geometricRelation upMovement = VectorMathsLibrary.RelationshipBetweenTwoPlanes(Vector3.up, movementDirection_normal);

        Vector3 rightDirection;
        Quaternion controllerOrientation_MovDirection;
        if (upMovement == GlobalVars.geometricRelation.parallelism)
        {
            rightDirection = Vector3.Cross(movementDirection_normal, control.forward);
            rightDirection = (Vector3.Dot(rightDirection, control.right) < 0) ? rightDirection * -1 : rightDirection;

            controllerOrientation_MovDirection = VectorMathsLibrary.RotationFromThreeVectors(Vector3.forward, rightDirection, control.forward, movementDirection_normal);
            controllerOrientation_MovDirection *= Quaternion.Euler(0, 0, 90f);
        }
        else
        {
            rightDirection = Vector3.Cross(movementDirection_normal, control.up);
            rightDirection = (Vector3.Dot(rightDirection, control.right) < 0) ? rightDirection * -1 : rightDirection;

            controllerOrientation_MovDirection = VectorMathsLibrary.RotationFromThreeVectors(Vector3.up, rightDirection, control.up, movementDirection_normal);
        }

        return controllerOrientation_MovDirection;
    }

    #endregion

    #region strokeCreation

    //new gameobject to keep new mesh
    private void NewStrokeGameobject(string strokeName, float ribbonWidth, int ringVertex, Color ribbonColor)
    {
        Debug.Log("Create Stroke");
        GameObject strokeGameobject = Instantiate(Resources.Load("Stroke"), Vector3.zero, Quaternion.identity) as GameObject;
        strokeGameobject.name = strokeName;
        currentCreatedStroke = strokeGameobject;

        if (GlobalVars.Instance.currentStrokeShape == "ribbon")
        {
            // Set brush to the flat ribbon.
            strokeGameobject.GetComponent<PlanarMeshCreation>().NewMesh(strokeName, ribbonWidth, ribbonColor);
        }

        if (GlobalVars.Instance.currentStrokeShape == "cylinder")
        {
            // Set brush to the cylinder
            strokeGameobject.GetComponent<CylinderMeshCreation>().NewMesh(strokeName, ribbonWidth, ringVertex, ribbonColor);
        }

        strokeGameobject.transform.SetParent(GameObject.Find("StrokeContainer").transform);

        //save new gameobject reference
        _currentStroke.SaveStrokeGameobject(strokeGameobject);
        strokeGameobject.SetActive(false);
    }

    //add vertex to mesh
    private void CreateMesh(Vector3 drawingPosition, Vector3 surfaceNormal, Matrix4x4 directionPose)
    {

        //Debug.Log(directionPose.ToString("G4"));
        //add points to the stroke

        bool exist = false; 
        if (GlobalVars.Instance.currentStrokeShape == "ribbon")
        {
            // Make mesh for ribbon
            exist = _currentStroke.stroke.GetComponent<PlanarMeshCreation>().CreateNormalMesh(drawingPosition, surfaceNormal); //need to change this for bool if needed
        }

        if (GlobalVars.Instance.currentStrokeShape == "cylinder")
        {
            // Make mesh for cylinder
            exist = _currentStroke.stroke.GetComponent<CylinderMeshCreation>().CreateCylinderMesh(drawingPosition, directionPose);
        }

        if (exist)
        {
            //save natural position
            _currentStrokeVertex++;
            _currentStroke.stroke.SetActive(true);
        } 
    }

    private void EndStrokeMesh(Vector3 finalPosition, Matrix4x4 directionPose)
    {

        if (GlobalVars.Instance.currentStrokeShape == "ribbon")
        {
            // End stroke for ribbon
            RibbonVertex lastVertex = _currentStroke.GetStrokePosition(_currentStrokeVertex);
            _currentStroke.stroke.GetComponent<PlanarMeshCreation>().LastCreateNormalMesh(lastVertex.position, finalPosition, lastVertex.ribbonNormal);
        }

        if (GlobalVars.Instance.currentStrokeShape == "cylinder")
        {
            // End stroke for cylinder
            _currentStroke.stroke.GetComponent<CylinderMeshCreation>().CreateLastCylinderMesh(finalPosition, directionPose, 3);
            _currentStrokeVertex++;
        }
    }

    #endregion
}