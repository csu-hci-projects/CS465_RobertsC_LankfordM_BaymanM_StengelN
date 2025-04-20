/* 
 * mayra b, 2017
 * basically to emulate the ondrawgizmo unity function but on the game window
 * not using the debug functions, because I don't think they are powerful enough for what I want.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode, ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
public class WidgetRenderer : MonoBehaviour {

    private GameObject widgetsContainer;
    private Dictionary<int,Widget> widgets;

    public Shader shaderObjects;
    private static Material materialObjects;

    public bool showScreenEffect = false;
    public  Material materialScreenEffects;

    public bool seeOnEditor = false;
    private GazeControll gazeControl;

    private void Awake()
    {
        EventManager.Instance.AddListener<CreateRayWidgetEvent>(CreateRayWidget);
        EventManager.Instance.AddListener<CreatePointWidgetEvent>(CreatePointWidget);
        EventManager.Instance.AddListener<CreateTextWidgetEvent>(CreateTextWidget);

        EventManager.Instance.AddListener<UpdateWidgetEvent>(UpdateWidget);

        EventManager.Instance.AddListener<DeleteVisualWidgetEvent>(DeleteVisualWidget);
        EventManager.Instance.AddListener<DeleteAllWidgetsEvent>(DeleteAllWidgets);
    }

    void Start () {

        widgetsContainer = GameObject.Find("WidgetContainer");

        widgets = new Dictionary<int, Widget>();

        materialObjects = new Material(shaderObjects);
        gazeControl = transform.GetComponent<GazeControll>();
    }

    private void CreateRayWidget(CreateRayWidgetEvent e)
    {
        if (!widgets.ContainsKey(e.name))
        {
            
            widgets.Add(e.name, new Widget(GlobalVars.WidgetType.ray, e.startPoint, e.color, e.direction, e.size));
          
        } else
        {

            UpdateWidget2(e.name, e.startPoint, e.direction, e.size, e.color);
            
        }      
    }

    private void CreatePointWidget(CreatePointWidgetEvent e)
    {

        if (!widgets.ContainsKey(e.name))
        {
            GameObject temp = Instantiate(Resources.Load("Point")) as GameObject;
            temp.transform.name = "w" + e.name;
            //temp.transform.parent = widgetsContainer.transform;
            temp.transform.position = e.startPoint;
            temp.transform.rotation = new Quaternion(0, 0, 0, 0);
            temp.transform.localScale = new Vector3(e.size, e.size, e.size);

            if (e.specialType)
            {
                Point thisPoint = temp.AddComponent(typeof(Point)) as Point;
                thisPoint.setPoint(e.name);

                temp.transform.SetParent(GameObject.Find("StrokeContainer").transform);
            }

            temp.GetComponent<MeshRenderer>().material.color = e.color;

            widgets.Add(e.name, new Widget(GlobalVars.WidgetType.sphere, e.startPoint, e.color, e.size));

            widgets[e.name].thisWidget = temp;
        }
        else
        {
            UpdateWidget2(e.name, e.startPoint, Vector3.zero, e.size, e.color);
        }
    }

    private void CreateTextWidget(CreateTextWidgetEvent e)
    {

        if (!widgets.ContainsKey(e.name))
        {
            GameObject temp = Instantiate(Resources.Load("Text")) as GameObject;
            temp.transform.name = "w" + e.name;
            temp.transform.parent = widgetsContainer.transform;
            temp.transform.position = e.position;
            temp.transform.rotation = new Quaternion(0, 0, 0, 0);

            temp.GetComponent<TextMesh>().text = e.text;
            temp.GetComponent<TextMesh>().characterSize = e.size;
            temp.GetComponent<TextMesh>().color = e.color;

            widgets.Add(e.name, new Widget(e.color, GlobalVars.WidgetType.text, e.position, e.size, e.text));
            widgets[e.name].thisWidget = temp;
        } else
        {
            UpdateWidget2(e.name, e.position, Vector3.zero, e.size,e.color);
        }
    }

    private void UpdateWidget(UpdateWidgetEvent e)
    {
        if (widgets.ContainsKey(e.name))
        {
            Widget w = widgets[e.name];

            w.position = e.position;
            w.direction = e.direction;

            if (w.thisWidget != null)
            {
                w.thisWidget.transform.position = e.position;
                w.thisWidget.transform.localScale = new Vector3(e.size, e.size, e.size);
            }
        }
    }

    private void UpdateWidget2(int name, Vector3 position, Vector3 direction, float size, Color color)
    {
       
        Widget w = widgets[name];

        w.position = position;
        w.direction = direction;

        if (w.thisWidget != null)
        {
            Debug.Log(name);
            w.thisWidget.transform.position = position;
            w.thisWidget.transform.localScale = new Vector3(size, size, size);
            w.thisWidget.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void DeleteVisualWidget(DeleteVisualWidgetEvent e)
    {
        Widget w;

        if (!widgets.TryGetValue(e.name, out w))
            return;

        //Widget w = widgets[e.name];

        if(w.thisWidget != null)
        {
            Destroy(w.thisWidget);
        }

        widgets.Remove(e.name);
        
    }

    public void DeleteAllWidgets(DeleteAllWidgetsEvent e)
    {
       
        if (widgetsContainer.transform.childCount > 0)
        {
            List<GameObject> children = new List<GameObject>();

            foreach (Transform child in widgetsContainer.transform)
            {
                children.Add(child.gameObject);
            }

            children.ForEach(child => Destroy(child));
        }

        widgets.Clear();
        
    }

    private void OnPostRender()
    {
        if (widgets != null)
        {
            if (widgets.Count > 0)
            {
                materialObjects.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(widgetsContainer.transform.transform.localToWorldMatrix);

                foreach (Widget w in widgets.Values)
                {
                    if (w.type == GlobalVars.WidgetType.ray)
                    {
                        //gl line
                        GL.Begin(GL.LINES);
                        GL.Color(w.color);

                        // Color newColor = new Color(w.color.r, w.color.g, w.color.b, 0.3f);
                        //GL.Color(newColor);

                        GL.Vertex3(w.position.x, w.position.y, w.position.z);

                        Vector3 endVertex = GeometricDetectionLibrary.PointLine(w.position, w.direction, w.size);
                        GL.Vertex3(endVertex.x, endVertex.y, endVertex.z);

                        GL.End();
                    }
                }

                GL.PopMatrix();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (seeOnEditor && widgets != null)
        {
            foreach (Widget w in widgets.Values)
            {
                if (w.type == GlobalVars.WidgetType.ray)
                {
                    Gizmos.color = w.color;
                    Gizmos.DrawRay(w.position, w.direction * w.size);
                }
            }
        }
    }
}
