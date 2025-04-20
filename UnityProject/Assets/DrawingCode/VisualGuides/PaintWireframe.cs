using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PaintWireframe : MonoBehaviour {

    private Vector3[] lines;
    private List<Vector3> linesArray;

    public Color color;

    private Material lineMaterial;
    public Shader shader;

	// Use this for initialization
	void Start () {

        lineMaterial = new Material(shader);

        linesArray = new List<Vector3>();

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        //for cube (I think)
        /*for(int i=0; i<mesh.vertices.Length / 3; i++)
        {
            linesArray.Add(mesh.vertices[i]);
        }*/
        
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            linesArray.Add(mesh.vertices[mesh.triangles[i * 3]]);
            linesArray.Add(mesh.vertices[mesh.triangles[i * 3 + 1]]);
            linesArray.Add(mesh.vertices[mesh.triangles[i * 3 + 2]]);
        }

        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnRenderObject()
    {

        GL.PushMatrix();

        lineMaterial.SetPass(0);

        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);

        for (int i = 0; i < linesArray.Count / 3; i++)
        {
            //if (i % 2 == 0) //for cube I think
            //{
                GL.Color(color);
                GL.Vertex(linesArray[i * 3]);
                GL.Vertex(linesArray[i * 3 + 1]);

                GL.Vertex(linesArray[i * 3 + 1]);
                GL.Vertex(linesArray[i * 3 + 2]);

                GL.Vertex(linesArray[i * 3 + 2]);
                GL.Vertex(linesArray[i * 3]);
            //}
        }

        //super lazy fix for a cube (NEED TO FIX THE FOR CYCLE)
        /*GL.Vertex(linesArray[20]);
        GL.Vertex(linesArray[14]);

        GL.Vertex(linesArray[14]);
        GL.Vertex(linesArray[6]);

        GL.Vertex(linesArray[6]);
        GL.Vertex(linesArray[20]);*/

        GL.End();
        GL.PopMatrix();
    }
}
