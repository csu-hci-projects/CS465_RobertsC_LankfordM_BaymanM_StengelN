/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{

    public GameObject CircleGuide;
    public GameObject StraightGuide;

    public StraightGuide straightGuideCode;
    public CircularGuide circularCode;

    public VisulizeExperiment visulizeExperimentForRumeysa;
    public VisulizeExperiment visulizeExperimentForParticipant;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void changeCondition()
    {
        GlobalVars.Instance.finishedConditionNumber++;
        GlobalVars.Instance.SetExperiment();


        if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.circle)
        {
            CircleGuide.active = true;
            circularCode.enabled = true;
            StraightGuide.active = false;
            straightGuideCode.enabled = false;
        }

        if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
        {
            CircleGuide.active = false;
            circularCode.enabled = false;
            StraightGuide.active = true;
            straightGuideCode.enabled = true;
        }


        if (GlobalVars.Instance.thisDrawnDirection == GlobalVars.DrawnDirection.depth)
        {

            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
            {
                straightGuideCode.rotateVertical();
            }
            else
            {

                circularCode.rotateVertical();
            }
        }

        if (GlobalVars.Instance.thisDrawnDirection == GlobalVars.DrawnDirection.lateral)
        {

            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
            {
                straightGuideCode.rotateHorizontal();
            }
            else
            {
                circularCode.rotateHorizontal();
            }
        }

        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.fullGuide)
        {

            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
                straightGuideCode.setFullMaterial();
            else
                circularCode.setFullMaterial();

        }

        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.dottedGuide)
        {
            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
                straightGuideCode.setDottedMaterial();
            else
                circularCode.setDottedMaterial();

        }

        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.noGuide)
        {
            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
                straightGuideCode.setNoMaterial();
            else
                circularCode.setNoMaterial();
        }

        if (GlobalVars.Instance.thisDrawnSize == GlobalVars.DrawnSize.large)
        {
            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
                straightGuideCode.setLargeSize();
            else
                circularCode.setLargeSize();
        }

        if (GlobalVars.Instance.thisDrawnSize == GlobalVars.DrawnSize.medium)
        {
            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
                straightGuideCode.setMediumSize();
            else
                circularCode.setMediumSize();
        }

        if (GlobalVars.Instance.thisDrawnSize == GlobalVars.DrawnSize.small)
        {
            if (GlobalVars.Instance.thisObjectShape == GlobalVars.ObjectShape.line)
                straightGuideCode.setSmallSize();
            else
                circularCode.setSmallSize();


        }

        visulizeExperimentForRumeysa.updateText();
        visulizeExperimentForParticipant.updateText();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeCondition();
        }

        
        if (Input.GetKeyDown(KeyCode.C))
        {
            CircleGuide.SetActive(true);
            StraightGuide.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CircleGuide.SetActive(false);
            StraightGuide.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GlobalVars.Instance.thisDrawnSize = GlobalVars.DrawnSize.small;
            straightGuideCode.setSmallSize();
            circularCode.setSmallSize();

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GlobalVars.Instance.thisDrawnSize = GlobalVars.DrawnSize.medium;
            straightGuideCode.setMediumSize();
            circularCode.setMediumSize();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GlobalVars.Instance.thisDrawnSize = GlobalVars.DrawnSize.large;
            straightGuideCode.setLargeSize();
            circularCode.setLargeSize();
        }
              
        if (Input.GetKeyDown(KeyCode.H))
        {
            straightGuideCode.rotateHorizontal();
            circularCode.rotateHorizontal();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            straightGuideCode.rotateVertical();
            circularCode.rotateVertical();
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.dottedGuide;
            straightGuideCode.setDottedMaterial();
            circularCode.setDottedMaterial();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.fullGuide;
            straightGuideCode.setFullMaterial();
            circularCode.setFullMaterial();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            GlobalVars.Instance.thisVisualGuide = GlobalVars.VisualGuide.noGuide;
            straightGuideCode.setNoMaterial();
            circularCode.setNoMaterial();
        }

    
    }
}*/
