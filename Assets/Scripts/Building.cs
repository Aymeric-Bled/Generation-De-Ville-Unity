using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
using Delaunay.Geo;

public class Building : MonoBehaviour {
    public static int counthigh = 0, countlow = 0;
    public float height = 30;
    public float width = 20;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;
    int[] trianglestop;
    /*
    public void SetMesh(List<Vector2> lp)
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.z);
        vertices = new Vector3[2*lp.Count+3];
        uv = new Vector2[vertices.Length];
        triangles = new int[6 * lp.Count];
        trianglestop = new int[3 * lp.Count];
        vertices[2 * lp.Count+2] = new Vector3(0, Chunk.GetHeight(center.x, center.y) + height + 5, 0);
        uv[2 * lp.Count+2] = Vector2.zero;
        float dist = 0;
        for (int i = 0; i < lp.Count + 1; i++)
        {
            int j = (i + 1) % lp.Count;
            //int k = (i - 1 + lp.Count) % lp.Count;
            // Contracting towards the center;
            int k = i % lp.Count;
            Vector2 p = 0.7f*lp[k];

            vertices[2 * i + 0] = new Vector3(p.x, Chunk.GetHeight(center.x + p.x, center.y + p.y) - 1, p.y);
            vertices[2 * i + 1] = new Vector3(p.x, Chunk.GetHeight(center.x + p.x, center.y + p.y) + height, p.y);
            int n = Mathf.Max(Mathf.FloorToInt(0.7f * (lp[k] - lp[j]).magnitude / width),1);
            uv[2 * i + 0] = new Vector2(dist, 0);
            uv[2 * i + 1] = new Vector2(dist, 1);
            dist += n;
            counthigh += n + 1;
            countlow += 1;
            if (i < lp.Count)
            {
                triangles[6 * i + 0] = 2 * i;
                triangles[6 * i + 1] = 2 * (i+1);
                triangles[6 * i + 2] = 2 * i + 1;
                triangles[6 * i + 3] = 2 * (i+1);
                triangles[6 * i + 4] = 2 * (i+1) + 1;
                triangles[6 * i + 5] = 2 * i + 1;
                trianglestop[3 * i + 0] = 2 * i + 1;
                trianglestop[3 * i + 1] = 2 * (i+1) + 1;
                trianglestop[3 * i + 2] = 2 * lp.Count+2;
            }
        }
    }
    */

    //MeshFilter mf;
    void Start()
    {



        GameObject go = GameObject.Find("Plane");
        //go.AddComponent<MeshFilter>();
        //go.AddComponent<MeshRenderer>();
        //Mesh mesh = go.GetComponent<MeshFilter>().mesh;

        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        //drawTriangle(mesh);
        MeshRenderer objRend = go.GetComponent<MeshRenderer>();
        Texture t = Resources.Load<Texture>("Facades/facade.jpg");
        //Vector2 size = new Vector2(1f / 100f, 1f / 100f);
        //objRend.material.SetTextureScale("_MainTex", size);
        objRend.material.mainTexture = t;
    }
    //This draws a triangle
    void drawTriangle(Mesh m)
    {
        //We need two arrays one to hold the vertices and one to hold the triangles
        Vector3[] VerteicesArray = new Vector3[6];
        int[] trianglesArray = new int[6];
        //lets add 3 vertices in the 3d space
        VerteicesArray[0] = new Vector3(0, 0, 0);
        VerteicesArray[1] = new Vector3(100, 0, 0);
        VerteicesArray[2] = new Vector3(0, 100, 0);
        VerteicesArray[3] = new Vector3(100, 100, 0);
        //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
        trianglesArray[0] = 0;
        trianglesArray[1] = 1;
        trianglesArray[2] = 2;
        trianglesArray[3] = 3;
        trianglesArray[4] = 2;
        trianglesArray[5] = 1;
        //add these two triangles to the mesh
        m.vertices = VerteicesArray;
        m.triangles = trianglesArray;

    }
}