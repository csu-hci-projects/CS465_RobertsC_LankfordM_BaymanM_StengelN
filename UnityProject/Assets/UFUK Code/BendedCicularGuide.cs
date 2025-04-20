/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendedCicularGuide : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public int linePrecision;
    public float radius;

    private void Awake()
    {
        GlobalVars.Instance.SetExperiment();
    }

    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = linePrecision;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        drawLine();
    }




    private void drawLine()
    {
        float alpha = Mathf.PI * 2 / (linePrecision - 1);

        for (int i = 0; i < linePrecision; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(Mathf.Sin(alpha * i), Mathf.Cos(alpha * i), Mathf.Sin(alpha * i) * Mathf.Sin(alpha * i)) * radius);

        }


    }
}*/
