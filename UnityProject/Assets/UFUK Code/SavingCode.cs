/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;
using Tobii.XR;
using System.Linq;
using System.IO;
using System;
using System.Xml;
using UnityEditor;

public class SavingCode : MonoBehaviour
{

    public Transform head;
    public Transform cursor;
    public Transform HeadToCursorGameObject;
    public Transform cursorToGazeGameObject;

    public Transform LineStartPointHead;
    public Transform LineStartPointGaze;
    public Transform LineStopPointHead;
    public Transform LineStopPointGaze;
    public Transform LineMidPointHead;
    public Transform LineMidPointGaze;
    public Transform LineQ1PointHead;
    public Transform LineQ1PointGaze;
    public Transform LineQ2PointHead;
    public Transform LineQ2PointGaze;

    public Transform CircleStartPointHead;
    public Transform CircleStartPointGaze;
    public Transform CircleStopPointHead;
    public Transform CircleStopPointGaze;
    public Transform CircleMidPointHead;
    public Transform CircleMidPointGaze;
    public Transform CircleQ1PointHead;
    public Transform CircleQ1PointGaze;
    public Transform CircleQ2PointHead;
    public Transform CircleQ2PointGaze;


    public StraightGuide straightGuide;
    public CircularGuide circularGuide;

    private float degreeBetweenLineStartPointVsHead;
    private float degreeBetweenLineStartPointVsGaze;
    private float degreeBetweenLineStopPointVsHead;
    private float degreeBetweenLineStopPointVsGaze;
    private float degreeBetweenLineMidPointVsHead;
    private float degreeBetweenLineMidPointVsGaze;
    private float degreeBetweenLineQ1PointVsHead;
    private float degreeBetweenLineQ1PointVsGaze;
    private float degreeBetweenLineQ2PointVsHead;
    private float degreeBetweenLineQ2PointVsGaze;

    private float degreeBetweenCircleStartPointVsHead;
    private float degreeBetweenCircleStartPointVsGaze;
    private float degreeBetweenCircleStopPointVsHead;
    private float degreeBetweenCircleStopPointVsGaze;
    private float degreeBetweenCircleMidPointVsHead;
    private float degreeBetweenCircleMidPointVsGaze;
    private float degreeBetweenCircleQ1PointVsHead;
    private float degreeBetweenCircleQ1PointVsGaze;
    private float degreeBetweenCircleQ2PointVsHead;
    private float degreeBetweenCircleQ2PointVsGaze;


    public float degreeBetweenHeadvsCursor;
    public float degreeBetweenHeadvsGaze;
    public float degreeBetweenCursorvsGaze;

    public float distanceBetweenHeadCursor;

    public float shortestDistanceToLineforCursor;

    public float shortestDistanceToLineforHead;

    public bool isTriggerpressed = false;

    private List<float> distances = new List<float>();
    private float startTime;

    private Vector3 startPos;
    private Vector3 stopPos;

    public CountFixations[] CircleFixations;
    public CountFixations[] LineFixations;
    public CountFixations CursorFixations;

    public VisulizeExperiment visulizeExperimentForRumeysa;
    public VisulizeExperiment visulizeExperimentForParticipant;


    private Vector3 rayOrigin; 
    private Vector3 rayDirection;

    string generalInfo;

    private string continousFileSave;
    private string PressedFileSave;
    private string endOfPressedFileSave;
    private string SceneFileSave;


    private string headData;
    private string gazeData;

    private float recorderID;


    // Start is called before the first frame update
    void Start()
    {

        string continousFileName =  GlobalVars.Instance.participantNumber.ToString() + "_" + DateTime.Now.ToString("h-mm-ss") + "_ContiniousFileSave.txt";
        string PressedFileName = GlobalVars.Instance.participantNumber.ToString() + "_" + DateTime.Now.ToString("h-mm-ss") + "_PressedFileSave.txt";
        string EndOfPressedFileName = GlobalVars.Instance.participantNumber.ToString() + "_" + DateTime.Now.ToString("h-mm-ss") + "_EndOfPressedFileSave.txt";
        string sceneFileName = GlobalVars.Instance.participantNumber.ToString() + "_" + DateTime.Now.ToString("h-mm-ss") + "_SceneFileSave.txt";


        continousFileSave = Application.dataPath + "/LOGs/Continous_" + continousFileName;
        PressedFileSave = Application.dataPath + "/LOGs/Pressed_" + PressedFileName;
        endOfPressedFileSave = Application.dataPath + "/LOGs/EndOfPressed_" + EndOfPressedFileName;
        SceneFileSave = Application.dataPath + "/LOGs/SceneFileName_" + EndOfPressedFileName;

        string message = "ParticipantNumber;FinishedCondition;ObjectShape;Direction;Size;VisualGuide;ID;degreeBetweenHeadvsCursor;degreeBetweenLineStartPointVsHead;degreeBetweenLineStopPointVsHead;degreeBetweenLineMidPointVsHead;"+
            "degreeBetweenLineQ1PointVsHead;degreeBetweenLineQ2PointVsHead;shortestDistanceToLineforCursor;shortestDistanceToLineforHead;distanceBetweenHeadCursor;Time;degreeBetweenHeadvsGaze;degreeBetweenLineStartPointVsGaze;"+
            "degreeBetweenLineStopPointVsGaze;degreeBetweenLineMidPointVsGaze;degreeBetweenLineQ1PointVsGaze;degreeBetweenLineQ2PointVsGaze;degreeBetweenCursorvsGaze;Time;RecorderID";

        using (StreamWriter sw = File.AppendText(continousFileSave))
        {
            sw.WriteLine(message);
        }

        using (StreamWriter sw = File.AppendText(PressedFileSave))
        {
            sw.WriteLine(message);
        }

        message = "ParticipantNumber;FinishedCondition;ObjectShape;Direction;Size;VisualGuide;ID;elapsedTime;averageDistance;STDistance1;STDistance2;startPos;stopPos;DrawingDistanceLine;StartFixations;Q1Fixations;MidFixations;Q2Fixations;EndFixations;CursorFixations;Time;RecorderID";

        using (StreamWriter sw = File.AppendText(endOfPressedFileSave))
        {
            sw.WriteLine(message);
        }

        message = "ParticipantNumber;FinishedCondition;ObjectShape;Direction;Size;VisualGuide;ID;cursorPosition;headPosition;headRotation;RayOrigin;rayDirection;" +
           "LineStartPointHead;LineStopPointHead;LineMidPointHead;LineQ1PointHead;LineQ2PointHead;CircleStartPointHead;CircleStopPointHead;" +
           "CircleMidPointHead;CircleQ1PointHead;CircleQ2PointHead;Time;recorderID";

        using (StreamWriter sw = File.AppendText(SceneFileSave))
        {
            sw.WriteLine(message);
        }
    }

    // Update is called once per frame
    void Update()
    {

        recorderID++;

        degreeBetweenHeadvsCursor = Quaternion.Angle(head.transform.rotation, HeadToCursorGameObject.transform.rotation);

        if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line) { 

        degreeBetweenLineStartPointVsHead = Quaternion.Angle(head.transform.rotation, LineStartPointHead.rotation);
        degreeBetweenLineStopPointVsHead = Quaternion.Angle(head.transform.rotation, LineStopPointHead.rotation);
        degreeBetweenLineMidPointVsHead = Quaternion.Angle(head.transform.rotation, LineMidPointHead.rotation);
        degreeBetweenLineQ1PointVsHead = Quaternion.Angle(head.transform.rotation, LineQ1PointHead.rotation);
        degreeBetweenLineQ2PointVsHead = Quaternion.Angle(head.transform.rotation, LineQ2PointHead.rotation);
        }
        else
        {
            degreeBetweenLineStartPointVsHead = Quaternion.Angle(head.transform.rotation, CircleStartPointHead.rotation);
            degreeBetweenLineStopPointVsHead = Quaternion.Angle(head.transform.rotation, CircleStopPointHead.rotation);
            degreeBetweenLineMidPointVsHead = Quaternion.Angle(head.transform.rotation, CircleMidPointHead.rotation);
            degreeBetweenLineQ1PointVsHead = Quaternion.Angle(head.transform.rotation, CircleQ1PointHead.rotation);
            degreeBetweenLineQ2PointVsHead = Quaternion.Angle(head.transform.rotation, CircleQ2PointHead.rotation);
        }



        distanceBetweenHeadCursor = Vector3.Distance(head.transform.position, cursor.transform.position);

        if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
        {

            shortestDistanceToLineforCursor = straightGuide.calculateDistanceForCursor();
            shortestDistanceToLineforHead = straightGuide.calculateDistanceForHead();
        }
        else
        {
            shortestDistanceToLineforCursor = circularGuide.calculateDistanceBetweenLineAndCursor();
            shortestDistanceToLineforHead = circularGuide.calculateDistanceBetweenLineAndHead();
        }

        distances.Add(shortestDistanceToLineforCursor);

        //Debug.Log(degreeBetweenHeadvsCursor + " " + degreeBetweenLineStartPointVsHead + " " + degreeBetweenLineStopPointVsHead + " " + degreeBetweenLineMidPointVsHead
        //    + " " + degreeBetweenLineQ1PointVsHead + " " + degreeBetweenLineQ2PointVsHead + " " + shortestDistanceToLineforCursor + " " + shortestDistanceToLineforHead + " " + distanceBetweenHeadCursor + " " + Time.time);

        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        if (eyeTrackingData.GazeRay.IsValid)
        {
            // The origin of the gaze ray is a 3D point
            Vector3 rayOrigin = eyeTrackingData.GazeRay.Origin;

            // The direction of the gaze ray is a normalized direction vector
            Vector3 rayDirection = eyeTrackingData.GazeRay.Direction;

            //Debug.Log(Quaternion.Angle(head.rotation, Quaternion.Euler(rayDirection)));
            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
            {

                degreeBetweenLineStartPointVsGaze = Quaternion.Angle(LineStartPointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineStopPointVsGaze = Quaternion.Angle(LineStopPointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineMidPointVsGaze = Quaternion.Angle(LineMidPointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineQ1PointVsGaze = Quaternion.Angle(LineQ1PointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineQ2PointVsGaze = Quaternion.Angle(LineQ2PointGaze.rotation, Quaternion.Euler(rayDirection));
            } else
            {
                degreeBetweenLineStartPointVsGaze = Quaternion.Angle(CircleStartPointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineStopPointVsGaze = Quaternion.Angle(CircleStopPointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineMidPointVsGaze = Quaternion.Angle(CircleMidPointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineQ1PointVsGaze = Quaternion.Angle(CircleQ1PointGaze.rotation, Quaternion.Euler(rayDirection));
                degreeBetweenLineQ2PointVsGaze = Quaternion.Angle(CircleQ2PointGaze.rotation, Quaternion.Euler(rayDirection));

            }
            degreeBetweenHeadvsGaze = Quaternion.Angle(head.rotation, Quaternion.Euler(rayDirection));
            degreeBetweenCursorvsGaze = Quaternion.Angle(cursorToGazeGameObject.transform.rotation, Quaternion.Euler(rayDirection));
            //   Debug.Log(degreeBetweenHeadvsGaze+ " " + degreeBetweenLineStartPointVsGaze + " " + degreeBetweenLineStopPointVsGaze + " " + degreeBetweenLineMidPointVsGaze
            //           + " " + degreeBetweenLineQ1PointVsGaze + " " + degreeBetweenLineQ2PointVsGaze + " " + degreeBetweenCursorvsGaze + " " + Time.time);

        }

        generalInfo = updateGeneralInfo();

        headData = degreeBetweenHeadvsCursor + ";" + degreeBetweenLineStartPointVsHead + ";" + degreeBetweenLineStopPointVsHead + ";" + degreeBetweenLineMidPointVsHead
            + ";" + degreeBetweenLineQ1PointVsHead + ";" + degreeBetweenLineQ2PointVsHead + ";" + shortestDistanceToLineforCursor + ";" + shortestDistanceToLineforHead + ";" + distanceBetweenHeadCursor + ";" + Time.time + ";";
        gazeData = degreeBetweenHeadvsGaze + ";" + degreeBetweenLineStartPointVsGaze + ";" + degreeBetweenLineStopPointVsGaze + ";" + degreeBetweenLineMidPointVsGaze
                            + ";" + degreeBetweenLineQ1PointVsGaze + ";" + degreeBetweenLineQ2PointVsGaze + ";" + degreeBetweenCursorvsGaze + ";" + Time.time;

        //Debug.Log(generalInfo + headData + gazeData);

        if (isTriggerpressed)
        {
            using (StreamWriter sw = File.AppendText(PressedFileSave))
            {
                sw.WriteLine(generalInfo+headData+ gazeData + ";" + recorderID);
            }
        }

        using (StreamWriter sw = File.AppendText(continousFileSave))
        {
            sw.WriteLine(generalInfo + headData + gazeData + ";" + recorderID);
        }

        string sceneData = cursor.transform.position.ToString("F4") + ";" + head.transform.position.ToString("F4") + ";" + head.transform.rotation.ToString("F4") + ";" + rayOrigin.ToString("F4") + ";" + rayDirection.ToString("F4") + ";" +
            LineStartPointHead.position.ToString("F4") + ";" + LineStopPointHead.position.ToString("F4") + ";" + LineMidPointHead.position.ToString("F4") + ";" + LineQ1PointHead.position.ToString("F4") + ";" + LineQ2PointHead.position.ToString("F4") + ";" + CircleStartPointHead.position.ToString("F4") + ";" + CircleStopPointHead.position.ToString("F4") + ";" +
            CircleMidPointHead.position.ToString("F4") + ";" + CircleQ1PointHead.position.ToString("F4") + ";" + CircleQ2PointHead.position.ToString("F4") + ";" + Time.time + ";" + recorderID ;

        using (StreamWriter sw = File.AppendText(SceneFileSave))
        {
            sw.WriteLine(generalInfo + sceneData);
        }

}

    public void triggerPressed(bool pressed)
    {
        isTriggerpressed = pressed;
    }

    public void TriggerPressedOnController()
    {

        startTime = Time.time;
        distances.Clear();
        distances = new List<float>();
        startPos = cursor.transform.position;
        CursorFixations.resetCounter();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(rayOrigin, rayDirection);

    }
    private string updateGeneralInfo()
    {
        string infoMessage= GlobalVars.Instance.participantNumber.ToString() + ";" +
            GlobalVars.Instance.finishedConditionNumber.ToString() + ";" +
            GlobalVars.Instance.thisObjectShape.ToString() + ";" +
            GlobalVars.Instance.thisDrawnDirection.ToString() + ";" +
            GlobalVars.Instance.thisDrawnSize.ToString() + ";" +
            GlobalVars.Instance.thisVisualGuide.ToString() + ";" +
            GlobalVars.Instance.drawinID.ToString() + ";";

        return infoMessage;

    }

    public void TriggerReleasedOnController()
    {

        GlobalVars.Instance.drawinID++;
        visulizeExperimentForRumeysa.updateText();
        visulizeExperimentForParticipant.updateText();

        float elapsedTime = Time.time - startTime;
        float averageDistance = distances.Average();
        float STDistance1 = Mathf.Sqrt(distances.Average(v => Mathf.Pow(v - averageDistance, 2)));
        float STDistance2 = STDistance1/(distances.Count-1);
        stopPos = cursor.transform.position;

        int StartFixations;
        int Q1Fixations;
        int MidFixations;
        int Q2Fixations;
        int EndFixations;
        int numberOfCursorFixations;

        if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
        {
            StartFixations = LineFixations[0].counter;
            Q1Fixations = LineFixations[1].counter;
            MidFixations = LineFixations[2].counter;
            Q2Fixations = LineFixations[3].counter;
            EndFixations = LineFixations[4].counter;

            numberOfCursorFixations = CursorFixations.counter;
            CursorFixations.resetCounter();
            foreach (CountFixations fixationsPoint in LineFixations)
            {
                fixationsPoint.resetCounter();
            }
        }
        else
        {
            StartFixations = CircleFixations[0].counter;
            Q1Fixations = CircleFixations[1].counter;
            MidFixations = CircleFixations[2].counter;
            Q2Fixations = CircleFixations[3].counter;
            EndFixations = CircleFixations[4].counter;

            numberOfCursorFixations = CursorFixations.counter;
            CursorFixations.resetCounter();

            foreach (CountFixations fixationsPoint in CircleFixations)
            {
                fixationsPoint.resetCounter();
            }

        }

        generalInfo = updateGeneralInfo();

        string messagePos = elapsedTime + ";" + averageDistance + ";" + STDistance1 + ";" + STDistance2 + ";" + startPos.ToString("F4") + ";" + stopPos.ToString("F4") + ";" + Vector3.Distance(startPos, stopPos) + ";";

        //Debug.Log(elapsedTime + " " + averageDistance + " " + STDistance1 + " " + STDistance2 + " " + startPos.ToString("F4") + " " + stopPos.ToString("F4") + " " + Vector3.Distance(startPos,stopPos));

        string fixations = StartFixations + ";" + Q1Fixations + ";" + MidFixations + ";" + Q2Fixations + ";" + EndFixations + ";" + numberOfCursorFixations + ";";

        //Debug.Log("Fixations: " + StartFixations + " " + Q1Fixations + " " + MidFixations + " " + Q2Fixations + " " + EndFixations + ";"); 

        string endOfStrokeMessage = generalInfo + messagePos + fixations + Time.time;

        //Debug.Log(endOfStrokeMessage);

        using (StreamWriter sw = File.AppendText(endOfPressedFileSave))
        {
            sw.WriteLine(endOfStrokeMessage + ";" + recorderID);
        }
        /*
        string strokeName = "stroke_" + GlobalVars.Instance.participantNumber.ToString() + "_" + GlobalVars.Instance.participantNumber.ToString() + "_" + GlobalVars.Instance.participantNumber.ToString()
    + "_" + GlobalVars.Instance.participantNumber.ToString() + "_" + GlobalVars.Instance.participantNumber.ToString()
    + "_" + e.meshName;

        string pathInsideAssets = "Assets/LOGs/study/" + GlobalVars.Instance.participantNumber.ToString();

        if (!Directory.Exists(pathInsideAssets))
        {
            Directory.CreateDirectory(pathInsideAssets);
        }

        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", pathInsideAssets, strokeName, "asset");
        path = FileUtil.GetProjectRelativePath(path);

        MeshUtility.Optimize(e.mesh);


        try
        {
            AssetDatabase.CreateAsset(e.mesh, path);
            AssetDatabase.SaveAssets();
        }
        catch (InvalidCastException eX)
        {
            Debug.Log(eX.ToString());
        }

        strokeName = "gameObject_" + e.participantName.ToString() + "_" + e.experimentVariables[0] + "_" + e.experimentVariables[1]
            + "_" + e.experimentVariables[2] + "_" + e.experimentVariables[3]
            + "_" + e.meshName;

        string nameLocalPath = pathInsideAssets + "/" + strokeName + ".prefab";

        UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(nameLocalPath);
        PrefabUtility.ReplacePrefab(GameObject.Find(e.meshName), prefab, ReplacePrefabOptions.ConnectToPrefab);

    
    }


}*/
