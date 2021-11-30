using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Delaunay;
using Delaunay.Geo;

public class StartRoads : MonoBehaviour
{

    public GameObject water;
    public GameObject route;
    public GameObject batiment;
    // Start is called before the first frame update
    public List<LineSegment> roads;
    public List<LineSegment> waterSegments;
    private List<Vector2> m_points;
    private List<LineSegment> m_edges = null;
    private List<LineSegment> m_spanningTree;
    private List<LineSegment> m_delaunayTriangulation;
    private List<uint> colors;



    void Start()
    {
        readFileRoads(@".\Python\roads1.txt", @".\Python\water1.txt");
        foreach (LineSegment segment in roads)
        {
            DrawRoad(segment, 0.1f, route, true);
            addBuilding(segment, 0.5f, 0.1f, true, 0.2f, 0.2f, 0.2f);
        }
        foreach (LineSegment segment in waterSegments)
        {
            DrawRoad(segment, 1f, water, true);
        }
        Voronoi();
    }

    void readFileRoads(string roadFile, string waterFile)
    {
        this.roads = new List<LineSegment>();
        this.waterSegments = new List<LineSegment>();
        string[] lines = System.IO.File.ReadAllLines(roadFile);
        foreach (string line in lines)
        {
            string[] points = line.Split('\t');
            Vector2 left = new Vector2(float.Parse(points[0]), float.Parse(points[1]));
            Vector2 right = new Vector2(float.Parse(points[2]), float.Parse(points[3]));
            roads.Add(new LineSegment(left, right));
        }
        lines = System.IO.File.ReadAllLines(waterFile);
        foreach (string line in lines)
        {
            string[] points = line.Split('\t');
            Vector2 left = new Vector2(float.Parse(points[0]), float.Parse(points[1]));
            Vector2 right = new Vector2(float.Parse(points[2]), float.Parse(points[3]));
            waterSegments.Add(new LineSegment(left, right));
        }
    }


    void DrawRoad(LineSegment segment, float width, GameObject gobject, bool doTranslation)
    {
        Vector2 left = (Vector2)segment.p0;
        Vector2 right = (Vector2)segment.p1;
        //Debug.Log("" + segment.p0);
        //Debug.Log("" + segment.p1);
        Vector3 vector = new Vector3((-left.y) * 0.05f + 5, 0, (-left.x) * 0.05f + 5);
        GameObject road = Instantiate(gobject, vector, Quaternion.identity);
        road.transform.localScale = new Vector3(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.05f, 0.03f, width);

        float theta = (left.y <= right.y) ? 180f - Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f : -Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f;

        road.transform.rotation = Quaternion.AngleAxis(theta, Vector3.up);
        if (doTranslation)
            road.transform.Translate(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f, 0, 0);
    }


    Boolean isValid()
    {
        return true;
    }

    void chooseRandomPoints(int length)
    {
        m_points = new List<Vector2>();
        colors = new List<uint>();
        System.Random ran = new System.Random();
        for (int i = 0; i < length;)
        {
            Vector2 point = new Vector2(((float)ran.NextDouble()) * 400.0f, ((float) ran.NextDouble()) * 400.0f);
            if (!m_points.Contains(point))
            {

                colors.Add((uint)1);
                m_points.Add(point);
                i++;
            }
        }
    }

    void Voronoi()
    {
        chooseRandomPoints(200);
        //Debug.Log("" + m_points.Count);
        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, 400, 400));
        m_edges = v.VoronoiDiagram();
        m_spanningTree = v.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();
        //Debug.Log("" + m_edges.Count);
        for (int i = 0; i < m_edges.Count; i++)
        {
            LineSegment seg = m_edges[i];
            DrawRoad(seg, 0.05f, route, true);
            addBuilding(seg, 0.5f, 0.05f, true, 0.1f, 0.1f, 0.2f);
        }
    }

    void addBuilding(LineSegment segment, float ratio, float roadWidth, bool direction, float width, float depth, float height)
    {
        Vector2 left = (Vector2)segment.p0;
        Vector2 right = (Vector2)segment.p1;
        Vector3 route = new Vector3(left.x - right.x, left.y - right.y, 0);
        Vector3 norm = Vector3.Normalize(Vector3.Cross(route, new Vector3(0, 0, 1)));

        float x = left.x + ratio * (right.x - left.x) - norm.x *30*roadWidth;
        float y = left.y + ratio * (right.y - left.y) - norm.y *30*roadWidth;


        left.x = x;
        left.y = y;
        Debug.Log("" + norm.z);
        right.x = left.x + depth * norm.x;
        right.y = left.y + depth * norm.y;

        GameObject building = Instantiate(batiment, new Vector3(-y *0.05f + 5,0.5f * height,-x * 0.05f + 5), Quaternion.identity);
        building.transform.localScale = new Vector3(depth, height, width);

        float theta = (left.y <= right.y) ? 180f - Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f : -Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f;

        building.transform.rotation = Quaternion.AngleAxis(theta, Vector3.up);
        building.transform.Translate(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f, 0, 0);

        //Vector2 position = left + ratio * (right - left) + norm * roadWidth / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
