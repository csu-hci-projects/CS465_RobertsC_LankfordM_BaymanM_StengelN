/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixGuide : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float height;
    public float linePrecision;
    public float radius;

    public float halfGuidePrecision;
    public float circleofNumber;

    public Material dotted;
    public Material fullMaterial;
    public Material noGuideMaterial;

    private void Awake()
    {
        GlobalVars.Instance.SetExperiment();
    }

    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        int numberOfPos = (int)linePrecision;
        lineRenderer.positionCount = numberOfPos;
        lineRenderer.material = dotted;
        drawLine();
        /*
        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.dottedGuide)
        {
            lineRenderer.material = dotted;
            drawLine();
        }


        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.fullGuide)
        {
            lineRenderer.material = fullMaterial;
            drawLine();
        }

        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.noGuide)
        {
            lineRenderer.material = noGuideMaterial;
            drawLine();
        }

    
    }

    

    public void changeColorWhenPressedDown()
    {
        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.noGuide)
        {
            lineRenderer.material = noGuideMaterial;
        }
    
    }


    public void changeColorWhenPressedUp()
    {

        if (GlobalVars.Instance.thisVisualGuide == GlobalVars.VisualGuide.noGuide)
        {
            lineRenderer.material = fullMaterial;
        }

    }


    public void changeMaterialFromOutside(GlobalVars.VisualGuide newGuide)
    {

        if (newGuide == GlobalVars.VisualGuide.dottedGuide)
        {
            lineRenderer.material = dotted;
        
        }


        if (newGuide == GlobalVars.VisualGuide.fullGuide)
        {
            lineRenderer.material = fullMaterial;

        }

        if (newGuide == GlobalVars.VisualGuide.noGuide)
        {
            lineRenderer.material = noGuideMaterial;
      
        }


    }


    private void drawLine()
    {
        float alpha = Mathf.PI * 2 / (linePrecision - 1);

        for (int i = 0; i < linePrecision; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(Mathf.Sin(alpha * i * circleofNumber), Mathf.Cos(alpha * i * circleofNumber), i* height / linePrecision) * radius);

        }





    }
}*/
