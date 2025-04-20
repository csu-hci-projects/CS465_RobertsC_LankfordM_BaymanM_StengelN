using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    GameObject StrokeContainer;
    GameObject TemporalStokeContainer;

    private bool _rotate;

    private bool _rightGripClick;
    private Vector3[] _rightController;//position, right, up, depth

    private bool _leftGripClick;
    private Vector3[] _leftController;//position, right, up, depth

    private Vector3[] startOrientation;//right, up, depth

    private Vector3 pivot;

    private bool oneAxisRotation = true;

    //TODO//
    //only 1 axisRotation
    //scale

    void Start () {

        StrokeContainer = GameObject.Find("StrokeContainer");

        _rightGripClick = false;
        _rightController = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        _leftGripClick = false;
        _leftController = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        startOrientation = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };

        pivot = Vector3.zero;
    }

    public void SetInitialValues(Dictionary<int, Stroke> allStrokes)
    {
        switch (GlobalVars.Instance.thisRotationType)
        {
            case GlobalVars.RotationType.twoHand:
                _rotate = (_rightGripClick && _leftGripClick) ? true : false;
                SetTwoControllerRotation();
                break;

            case GlobalVars.RotationType.oneHand:
                _rotate = (_leftGripClick) ? true : false;
                SetOneControllerRotation();
                break;

            default:
                _rotate = false;
                Debug.Log("walk");
                break;
        }

        if (_rotate)
        {
            if (TemporalStokeContainer == null)
            {
                TemporalStokeContainer = CreateTemporalContainer();

                foreach (Stroke stroke in allStrokes.Values)
                {
                    stroke.stroke.transform.SetParent(TemporalStokeContainer.transform);
                }
            }
        }
    }

    public void FinishRotation(Dictionary<int, Stroke> allStrokes)
    {
        _rotate = false;

        if (TemporalStokeContainer != null)
        {
            float distanceController = VectorMathsLibrary.DistanceBetweenTwoPoints(pivot, TemporalStokeContainer.transform.position);
            float angle = VectorMathsLibrary.AngleBetweenTwoPlanes(startOrientation[2], TemporalStokeContainer.transform.forward);

            string position = "" + TemporalStokeContainer.transform.position.x + "," + TemporalStokeContainer.transform.position.y + "," + TemporalStokeContainer.transform.position.z;
            string rotation = "" + TemporalStokeContainer.transform.rotation.x + "," + TemporalStokeContainer.transform.rotation.y + "," + TemporalStokeContainer.transform.rotation.z + "," + TemporalStokeContainer.transform.rotation.w;
            string viewNormal = TemporalStokeContainer.transform.forward.x + "," + TemporalStokeContainer.transform.forward.y + "," + TemporalStokeContainer.transform.forward.z;

            string rotationValues = distanceController + "/" + angle + "/" + position + "/" + rotation + "/" + viewNormal;

            /*EventManager.Instance.TriggerEvent(new CreateEntryEvent(GlobalVars.Instance.currentParticipant, GlobalVars.SpatialAbility.none, GlobalVars.Instance.currentStudy,
               new string[] { GlobalVars.Instance.thisObjectShape.ToString(), GlobalVars.Instance.thisRotationType.ToString() },
               GlobalVars.LogCategory.newRotation, "" + rotationValues));*/

            foreach (Stroke stroke in allStrokes.Values)
            {
                stroke.stroke.transform.SetParent(StrokeContainer.transform);
            }

            Destroy(TemporalStokeContainer);
        }
    }

    #region rotation/translation
    private void SetTwoControllerRotation()
    {
        pivot = VectorMathsLibrary.MiddlePoint(_leftController[0], _rightController[0]);

        startOrientation[0] = VectorMathsLibrary.DirectionBetweenPoints(_leftController[0], _rightController[0]);
        startOrientation[0] = (Vector3.Dot(_leftController[1], startOrientation[0]) < 0) ? -startOrientation[0] : startOrientation[0];
        startOrientation[1] = VectorMathsLibrary.MiddlePoint(_leftController[2], _rightController[2]);

        if (oneAxisRotation)
        {
            Vector3 pointInControllerDirection = GeometricDetectionLibrary.PointLine(pivot, startOrientation[0], 0.5f);
            Vector3 pointInHorizonPlane = VectorMathsLibrary.ProjectPointToPlane(pointInControllerDirection, Vector3.up, pivot);

            startOrientation[0] = VectorMathsLibrary.DirectionBetweenPoints(pivot, pointInHorizonPlane);
            startOrientation[1] = Vector3.up;
        }

        startOrientation[2] = Vector3.Cross(startOrientation[0], startOrientation[1]);
    }

    private void SetOneControllerRotation()
    {
        pivot = _leftController[0];

        if (oneAxisRotation)
        {
            Vector3 pointInControllerDirection = GeometricDetectionLibrary.PointLine(pivot, _leftController[3], 0.5f);
            Vector3 pointInHorizonPlane = VectorMathsLibrary.ProjectPointToPlane(pointInControllerDirection, Vector3.up, pivot);

            startOrientation[2] = VectorMathsLibrary.DirectionBetweenPoints(_leftController[0], pointInHorizonPlane);
            startOrientation[2] = (Vector3.Dot(_leftController[3], startOrientation[2]) < 0) ? -startOrientation[2] : startOrientation[2];

            startOrientation[1] = Vector3.up;

            startOrientation[0] = Vector3.Cross(startOrientation[2], startOrientation[1]);
            startOrientation[0] = (Vector3.Dot(_leftController[1], startOrientation[0]) < 0) ? -startOrientation[0] : startOrientation[0];
        }
        else
        {
            startOrientation[0] = _leftController[1];
            startOrientation[1] = _leftController[2];
            startOrientation[2] = _leftController[3];
        }
    }

    public void TwoHandRotateObject(Transform controllerRight)
    {
        if (_rotate)
        {
            Vector3 currentPivot = VectorMathsLibrary.MiddlePoint(_leftController[0], controllerRight.position);

            //rotation
            Vector3 currentRight = VectorMathsLibrary.DirectionBetweenPoints(_leftController[0], controllerRight.position);
            currentRight = (Vector3.Dot(controllerRight.right, currentRight) < 0) ? -currentRight : currentRight;

            Vector3 currentUp = VectorMathsLibrary.MiddlePoint(_leftController[2], controllerRight.up);

            if (oneAxisRotation)
            {
                Vector3 pointInControllerDirection = GeometricDetectionLibrary.PointLine(currentPivot, currentRight, 0.5f);
                Vector3 pointInHorizonPlane = VectorMathsLibrary.ProjectPointToPlane(pointInControllerDirection, Vector3.up, currentPivot);

                currentRight = VectorMathsLibrary.DirectionBetweenPoints(currentPivot, pointInHorizonPlane);
                currentUp = Vector3.up;
            }

            Vector3 currentDepth = Vector3.Cross(currentRight, currentUp);

            Quaternion newRotation = Quaternion.LookRotation(currentDepth, currentUp);
            TemporalStokeContainer.transform.rotation = newRotation;

            //translation          
            TemporalStokeContainer.transform.position = currentPivot;
        }
    }

    public void OneHandRotateObject()
    {
        if (_rotate)
        {
            //rotation
            Vector3 currentUp = _leftController[2];
            Vector3 currentDepth = _leftController[3];

            if (oneAxisRotation)
            {
                Vector3 pointInControllerDirection = GeometricDetectionLibrary.PointLine(_leftController[0], currentDepth, 0.5f);
                Vector3 pointInHorizonPlane = VectorMathsLibrary.ProjectPointToPlane(pointInControllerDirection, Vector3.up, _leftController[0]);

                currentDepth = VectorMathsLibrary.DirectionBetweenPoints(_leftController[0], pointInHorizonPlane);
                currentDepth = (Vector3.Dot(_leftController[3], currentDepth) < 0) ? -currentDepth : currentDepth;

                currentUp = Vector3.up;
            }

            Quaternion newRotation = Quaternion.LookRotation(currentDepth, currentUp);
            TemporalStokeContainer.transform.rotation = newRotation;

            //translation
            TemporalStokeContainer.transform.position = _leftController[0];
        }
    }
    #endregion

    //TODO!!!! NOTHING HERE THAT WORKS
    #region scale
    public void TwoHandScale()
    {

        /*         distanceController = VectorMathsLibrary.DistanceBetweenTwoPoints(_leftController[0], _rightController[0]);
         *             float distanceMoved = VectorMathsLibrary.DistanceBetweenTwoPoints(pivot, _leftController[0]);
         * Vector3 movementDirection = VectorMathsLibrary.DirectionBetweenPoints(currentPivot, pivot).normalized;
        float distanceMoved = VectorMathsLibrary.DistanceBetweenTwoPoints(pivot, currentPivot);

        EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(1, pivot, Color.yellow, 0.05f));
        EventManager.Instance.QueueEvent(new CreatePointWidgetEvent(2, currentPivot, Color.green, 0.05f));
        Debug.Log(distanceMoved);
        */
    }
    #endregion

    private GameObject CreateTemporalContainer()
    {
        GameObject temp = new GameObject("RotationObject");
        temp.transform.position = pivot;

        temp.transform.rotation = new Quaternion();
        temp.transform.rotation = Quaternion.LookRotation(startOrientation[2], Vector3.up);

        return temp;
    }

    #region accesors

    public void SetRightController(Transform controller)
    {
        _rightController[0] = controller.position;
        _rightController[1] = controller.right;
        _rightController[2] = controller.up;
        _rightController[3] = controller.forward;
    }

    public void SetLeftController(Transform controller)
    {
        _leftController[0] = controller.position;
        _leftController[1] = controller.right;
        _leftController[2] = controller.up;
        _leftController[3] = controller.forward;
    }

    public bool rightGripClick
    {
        set { _rightGripClick = value; }
    }

    public bool leftGripClick
    {
        set { _leftGripClick = value; }
    }

    #endregion
}
