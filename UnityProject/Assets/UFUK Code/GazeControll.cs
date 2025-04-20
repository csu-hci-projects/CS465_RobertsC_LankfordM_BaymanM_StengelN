using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Tobii.G2OM;
//using Tobii.XR;
 

public class GazeControll : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 rayOriginV3;
    private Vector3 rayDirectionV3;
    private Vector3 rayStartPoint;
    private Vector3 rayDirection;

    public Vector3 test;
    public Transform mySphere;
    public bool enableFilter;

    GameObject myGameObject;
    OneEuroFilter floatFilter;
    OneEuroFilter<Vector3> vector3Filter;
    public float filterFrequency = 120.0f;
    private Vector3 filteredInput;


    private void Start()
    {
        myGameObject = new GameObject();
        floatFilter = new OneEuroFilter(filterFrequency);
        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency);
        enableFilter = true;
    }


    //public Transform UpdateGazeInfo()
    //{
    //    rayStartPoint = getGazePosition();
    //    rayDirection = getEyeForward();
    //    //Debug.Log(rayStartPoint);
    //    if (enableFilter)
    //    {
    //        rayDirection = vector3Filter.Filter(rayDirection);
    //    }
    //    Ray ray = new Ray(rayStartPoint, rayDirection);
    //    myGameObject.transform.position = rayStartPoint;
    //    mySphere.position = ray.GetPoint(0.75f);

    //    return mySphere;
    //}

    public Vector3 getStartGaze()
    {
        return rayStartPoint;
    }

    public Vector3 getGazeRayDirection()
    {
        return rayDirectionV3;
    }

    //public Vector3 getGazePosition()
    //{
    //     //var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);


    //    // Check if gaze ray is valid
    //    if (eyeTrackingData.GazeRay.IsValid)
    //    {

    //        // The origin of the gaze ray is a 3D point   
    //       var rayOrigin1 = eyeTrackingData.GazeRay.Origin;
            
    //        // The direction of the gaze ray is a normalized direction vector
    //       //var rayDirection1 = eyeTrackingData.GazeRay.Direction;

    //        return rayOriginV3 = rayOrigin1;
    //        /*
    //        var transform = CameraHelper.GetCameraTransform(); // You could also use Camera.main.transform
      
    //        var rayOrigin = transform.TransformPoint(rayOrigin1);

            
    //        Debug.Log(rayOrigin.ToString("F4"));
    //        return rayOrigin;
    //        */
    //    }
    //    else
    //    {
    //        //Debug.Log("Gaze zero");

    //        return Vector3.zero;
    //    }
    //}

    //public Vector3 getEyeForward()
    //{

    //    var eyeTrackingDataLocal = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);

    //    var eyesDirection = eyeTrackingDataLocal.GazeRay.Direction;
    //    var transform = CameraHelper.GetCameraTransform(); // You could also use Camera.main.transform

    //    // return eyesDirection;
    //    Vector3 rayDirectionV3 = transform.TransformDirection(eyesDirection);
    //    return rayDirectionV3;
    //}


    public void OnDrawGizmos()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(rayOriginV3, rayDirectionV3);
    }
}

