/* 
 * mayra barrera, 2017
 * variables accessible everywhere
 * most of them are static variables,
 * others are me being lazy
 */


using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml;

public class GlobalVars {

    private static GlobalVars _instance;
    public static GlobalVars Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GlobalVars();
            return _instance;
        }
    }

    public string currentParticipant;
    public int currentStudy;

    public Dictionary<string, List<LogPosition>> userHeadPositions = new Dictionary<string, List<LogPosition>>();
    public Dictionary<string, List<LogPosition>> userRightHandPositions = new Dictionary<string, List<LogPosition>>();
    public Dictionary<string, List<LogPosition>> userLeftHandPositions = new Dictionary<string, List<LogPosition>>();

    public VisualGuide thisVisualGuide;
    public SmartGuide thisSmartGuide;
    public ObjectShape thisObjectShape;
    public RotationType thisRotationType;
    public bool useEyeTracking;

    //public int phase
    public ExperimentPhase thisExperiment;

    #region experimentalVariables
    public enum SpatialAbility
    {
        none,
        high,
        low
    }

    public static SpatialAbility GetSpatialAbility(int study, int participantName)
    {
        SpatialAbility mySPA;

        if (study == 1)
        {
            switch (participantName)
            {
               
                case 4:
                case 6:              
                case 8:
                case 10:
                case 11:
                case 12:
                    mySPA = SpatialAbility.low;
                    break;               
                case 5:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                    mySPA = SpatialAbility.high;
                    break;
                default:
                case 1:
                case 2:
                case 3:
                case 7:
                case 9:
                    mySPA = SpatialAbility.none;
                    break;
            }
        }

        else
        {
            switch (participantName)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:     
                case 12:       
                default:
                    mySPA = SpatialAbility.none;
                    break;
            }
        }

        return mySPA;
    }

    public enum ObjectShape
    {
        none,
        shape1,
        shape2,
        shape3,
        arqShape1,
        shape4,
        test
    }

    public enum VisualGuide
    {
        none,
        SmartGuides,
        Templates     
    }

    public enum SmartGuide
    {
        none,
        frustum,
        glow,
        grid,
        autocomplete,
        crosshair,
        gazecrosshair,
        lines
    }

    public enum RotationType
    {
        none,
        walk,
        twoHand,
        oneHand
    }
    #endregion

    public enum ExperimentPhase
    {
        seeModel,
        drawing,
        finish
    }

    #region user interaction

    public Color currentStrokeColor;
    public float currentStrokeSize;
    public bool currentStrokeDelete;
    public string currentStrokeShape = "cylinder"; // default stroke to cylinder for now
    public List<GameObject> undoList = new List<GameObject>();
    public List<GameObject> redoList = new List<GameObject>();
    public int consecUndoCount = 0; 

    public float currentStrokeOutlineSize;

    public static float safeAreaRadius = 0.1f;
    public static float visibleGridRadius = 0.3f;

    public static int   gridLines = 20;
    public static float gridLength = 4;

    public static float safeDeviationAngle = 10; //in degrees

    #endregion

    #region log

    public XmlWriterSettings fragmentSetting;
    public FileStream logFile;

    public int widgetCount;

    public enum WidgetType
    {
        ray,
        sphere,
        text
    }

    public enum LogCategory
    {
        none,
        controlPosition,
        controlRotation,
        leftControllerPose,
        rightControllerPose,
        userPosition,
        userRotation,
        userPose,
        newStroke,
        endStroke,
        startTry,
        endTry,
        changeView,
        deviationAngleX,
        deviationAngleY,
        deviationAngleZ,
        newRotation,
        templateCreated,
        templateSet,
        templateDelete
    }
  
    #endregion

    #region legacyStuff

    public static float snappingDistanceThreshold = 0.005f;
    public static float segmentChangeDistance = 0.03f;//TUNE THIS WHEN READY!!!

    public enum geometricRelation
    {
        none,
        parallelism,
        perpendicularity,
        acute45
    }

    public enum shapeType
    {
        none,
        line,
        circle
    }

    #endregion
}
