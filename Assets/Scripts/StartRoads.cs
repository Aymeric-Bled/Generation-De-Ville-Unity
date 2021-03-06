using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEditor;
using System;
using Delaunay;
using Delaunay.Geo;

public class StartRoads : MonoBehaviour
{

    public GameObject water;
    public GameObject smallRoad;
    public GameObject majorRoad;
    public GameObject intersection;
    public NavMeshSurface plane;
    public NavMeshAgent cameraCar;

    public List<Tuple<float,LineSegment>> roads = new List<Tuple<float, LineSegment>>();
    public List<Tuple<float, LineSegment>> majorRoads = new List<Tuple<float, LineSegment>>();
    public List<Tuple<float, LineSegment>> waterSegments = new List<Tuple<float, LineSegment>>();
    private List<Vector2> m_points;
    private List<LineSegment> m_edges = null;
    private List<LineSegment> m_spanningTree;
    private List<LineSegment> m_delaunayTriangulation;
    private List<uint> colors;
    private System.Random rand = new System.Random();
    private List<NavMeshAgent> cars = new List<NavMeshAgent>();



    // Start is called before the first frame update
    void Start()
    {
        readFileRoads(@".\Python\roads1.txt", @".\Python\water.txt");
        Voronoi(300, 0.1f);
        splitRoads();
        removeRoadsOnWater();
        foreach (Tuple<float, LineSegment> segment in majorRoads)
        {
            DrawRoad(segment, majorRoad, true);
            addCirclesAtIntersections(segment, intersection);
        }
        foreach (Tuple<float, LineSegment> segment in roads)
        {
            DrawRoad(segment, smallRoad, true);
            addBuildings(segment, true, 0.1f, 0.1f, 0.2f, 0.2f, 0.2f, 0.4f); 
            addBuildings(segment, false, 0.1f, 0.1f, 0.2f, 0.2f, 0.2f, 0.4f);

        }
        foreach (Tuple<float, LineSegment> segment in waterSegments)
        {
            DrawRoad(segment, water, true);
        }

        plane = GetComponent<NavMeshSurface>();
        plane.BuildNavMesh();


        
        cars.Add(cameraCar);

        NavMeshHit closestHit;
        int index = rand.Next(roads.Count);
        Tuple<float, LineSegment> road = roads[index];
        LineSegment s = road.Item2;
        Vector2 left = (Vector2)s.p0;
        Vector2 right = (Vector2)s.p1;
        Vector3 position = new Vector3(((-left.y) * 0.5f * 0.05f + 5) * 10f, 0f, ((-left.x) * 0.5f * 0.05f + 5) * 10f);
        cameraCar.transform.position = position;
        index = rand.Next(roads.Count);
        road = roads[index];
        s = road.Item2;
        left = (Vector2)s.p0;
        right = (Vector2)s.p1;
        Vector3 destPosition = new Vector3(((-left.y) * 0.5f * 0.05f + 5) * 10f, 0f, ((-left.x) * 0.5f * 0.05f + 5) * 10f);
        cameraCar.destination = destPosition;
        List<GameObject> prefabs = new List<GameObject>();
        prefabs.Add((GameObject) Resources.Load("Simple Vehicle Pack/Prefabs/Bus_1"));
        prefabs.Add((GameObject)Resources.Load("Simple Vehicle Pack/Prefabs/Bus_2"));
        prefabs.Add((GameObject)Resources.Load("Simple Vehicle Pack/Prefabs/Car_1"));
        prefabs.Add((GameObject)Resources.Load("Simple Vehicle Pack/Prefabs/Car_2"));
        prefabs.Add((GameObject)Resources.Load("Simple Vehicle Pack/Prefabs/Car_3"));
        prefabs.Add((GameObject)Resources.Load("Simple Vehicle Pack/Prefabs/Car_4"));
        prefabs.Add((GameObject)Resources.Load("Simple Vehicle Pack/Prefabs/Taxi"));
        foreach (Tuple<float, LineSegment> segment in roads)
        {
            
            s = segment.Item2;
            left = (Vector2)s.p0;
            right = (Vector2)s.p1;
            Vector3 sourcePosition = new Vector3(((-left.y - right.y) * 0.5f * 0.05f + 5) * 10f, 0f, ((-left.x - right.x) * 0.5f * 0.05f + 5) * 10f);
            NavMesh.SamplePosition(sourcePosition, out closestHit, 500, 1);
            int index2 = rand.Next(prefabs.Count);
            GameObject car = prefabs[index2];
            GameObject c = Instantiate(car, closestHit.position, Quaternion.Euler(0, 0, 0));
            NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
            cars.Add(agent);
            index = rand.Next(roads.Count);
            road = roads[index];
            s = road.Item2;
            left = (Vector2)s.p0;
            right = (Vector2)s.p1;
            destPosition = new Vector3(((-left.y - right.y) * 0.5f * 0.05f + 5) * 10f, 0f, ((-left.x - right.x) * 0.5f * 0.05f + 5) * 10f);
            NavMesh.SamplePosition(destPosition, out closestHit, 500, 1);
            agent.destination = closestHit.position;
        }
    }

    void Update()
    {

        
        foreach (NavMeshAgent c in cars)
        {

            if (Vector3.Distance(c.nextPosition, c.destination) < 1f || c.isStopped)
            {
                int index = rand.Next(roads.Count);
                Tuple<float, LineSegment> road = roads[index];
                LineSegment s = road.Item2;
                Vector2 left = (Vector2)s.p0;
                Vector2 right = (Vector2)s.p1;
                Vector3 destPosition = new Vector3(((-left.y) * 0.5f * 0.05f + 5) * 10f, 0.05f, ((-left.x) * 0.5f * 0.05f + 5) * 10f);
                c.SetDestination(destPosition);
                NavMeshPath path = new NavMeshPath();
                c.CalculatePath(destPosition, path);
                c.SetPath(path);
            }
        }
    }

    void readFileRoads(string roadFile, string waterFile)
    {
        string[] lines = System.IO.File.ReadAllLines(roadFile);
        foreach (string line in lines)
        {
            string[] points = line.Split('\t');
            Vector2 left = new Vector2(float.Parse(points[0]), float.Parse(points[1]));
            Vector2 right = new Vector2(float.Parse(points[2]), float.Parse(points[3]));
            majorRoads.Add(new Tuple<float, LineSegment>(0.2f, new LineSegment(left, right)));
        }
        lines = System.IO.File.ReadAllLines(waterFile);
        foreach (string line in lines)
        {
            string[] points = line.Split('\t');
            Vector2 left = new Vector2(float.Parse(points[0]), float.Parse(points[1]));
            Vector2 right = new Vector2(float.Parse(points[2]), float.Parse(points[3]));
            waterSegments.Add(new Tuple<float, LineSegment>(0.5f, new LineSegment(left, right)));
        }
    }


    void DrawRoad(Tuple<float, LineSegment> route, GameObject gobject, bool doTranslation)
    {
        float width = route.Item1;
        LineSegment segment = route.Item2;
        Vector2 left = (Vector2)segment.p0;
        Vector2 right = (Vector2)segment.p1;
        Vector3 vector = new Vector3(((-left.y) * 0.05f + 5 )* 10f, 0, ((-left.x) * 0.05f + 5 )* 10f);

        GameObject road = Instantiate(gobject, vector, Quaternion.identity);
        if (gobject == water) {
            road.transform.localScale = new Vector3(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.05f * 10f, 0.01f, width * 10f);
        }
        else
        {
            road.transform.localScale = new Vector3(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.05f * 10f, 0.05f, width * 10f);
        }

        float theta = (left.y <= right.y) ? 180f - Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f : -Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f;

        road.transform.rotation = Quaternion.AngleAxis(theta, Vector3.up);

        if (doTranslation)
            road.transform.Translate(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f * 10f, 0, 0);
        
    }

    void addCirclesAtIntersections(Tuple<float, LineSegment> route, GameObject gobject)
    {
        float width = route.Item1;
        LineSegment segment = route.Item2;
        Vector2 left = (Vector2)segment.p0;
        Vector2 right = (Vector2)segment.p1;
        Vector3 vector = new Vector3(((-left.y) * 0.05f + 5) * 10f, 0, ((-left.x) * 0.05f + 5) * 10f);
        GameObject intersection1 = Instantiate(gobject, vector, Quaternion.identity);
        intersection1.transform.localScale = new Vector3(width * 10f, 0.025f, width * 10f);
        vector = new Vector3(((-right.y) * 0.05f + 5) * 10f, 0, ((-right.x) * 0.05f + 5) * 10f);
        GameObject intersection2 = Instantiate(gobject, vector, Quaternion.identity);
        intersection2.transform.localScale = new Vector3(width * 10f, 0.025f, width * 10f);
    }


    Boolean isValid()
    {
        return true;
    }

    void chooseRandomPoints(int length)
    {
        m_points = new List<Vector2>();
        colors = new List<uint>();
        for (int i = 0; i < length;)
        {
            Vector2 point = new Vector2(((float)rand.NextDouble()) * 400.0f, ((float) rand.NextDouble()) * 400.0f);
            if (!m_points.Contains(point))
            {

                colors.Add((uint)1);
                m_points.Add(point);
                i++;
            }
        }
    }

    void Voronoi(int count, float width)
    {
        chooseRandomPoints(count);
        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, 400, 400));
        m_edges = v.VoronoiDiagram();
        m_spanningTree = v.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();
        for (int i = 0; i < m_edges.Count; i++)
        {
            LineSegment seg = m_edges[i];
            roads.Add(new Tuple<float, LineSegment>(width, seg));
        }
    }


    void addBuildings(Tuple<float, LineSegment> road, bool direction, float minWidth, float minDepth, float minHeight, float maxWidth, float maxDepth, float maxHeight)
    {
        LineSegment segment = road.Item2;
        float roadWidth = road.Item1;
        Vector2 left = (Vector2)segment.p0;
        Vector2 right = (Vector2)segment.p1;
        float length = Vector2.Distance(left, right) * 0.05f * 10f;
        float intersectionSpace = (maxWidth > maxDepth) ? maxWidth : maxDepth;
        intersectionSpace *= 2;
        intersectionSpace += roadWidth;
        //Debug.Log("LENGTH: " + length);
        //Debug.Log("WIDTH: " + intersectionSpace);
        float rate = 5 * intersectionSpace / length;
        while (rate < 1f - 10 * intersectionSpace / length)
        {
            float width = (float) (minWidth + rand.NextDouble() * (maxWidth - minWidth));
            float depth = (float)(minDepth + rand.NextDouble() * (maxDepth - minDepth));
            float height = (float)(minHeight + rand.NextDouble() * (maxHeight - minHeight));
            rate += 10 * (width + 0.05f) / length;
            addBuilding(road, rate, direction, width, depth, height);
        }
    }

    void addBuilding(Tuple<float, LineSegment> road, float ratio , bool direction, float width, float depth, float height)
    {

        List<GameObject> prefabs = new List<GameObject>();
        prefabs.Add((GameObject)Resources.Load("Batiment/Batiment 1"));
        prefabs.Add((GameObject)Resources.Load("Batiment/Batiment 2"));
        prefabs.Add((GameObject)Resources.Load("Batiment/Batiment 3"));
        prefabs.Add((GameObject)Resources.Load("Batiment/Batiment 4"));
        prefabs.Add((GameObject)Resources.Load("Batiment/Batiment 5"));
        int index = rand.Next(prefabs.Count);
        GameObject batiment = prefabs[index];
        float roadWidth = road.Item1;
        LineSegment segment = road.Item2;
        Vector2 left = (Vector2)segment.p0;
        Vector2 right = (Vector2)segment.p1;
        Vector3 route = new Vector3(left.x - right.x, left.y - right.y, 0);
        Vector3 norm = Vector3.Normalize(Vector3.Cross(route, new Vector3(0, 0, 1)));
        float x = 0, y = 0;
        if (direction)
        {
            x = left.x + ratio * (right.x - left.x) + norm.x * 10 * (depth + roadWidth);
            y = left.y + ratio * (right.y - left.y) + norm.y * 10 * (depth + roadWidth);
        }

        else
        {
            x = left.x + ratio * (right.x - left.x) - norm.x * 10 * (depth + roadWidth);
            y = left.y + ratio * (right.y - left.y) - norm.y * 10 * (depth + roadWidth);
        }


        left.x = x;
        left.y = y;
        right.x = left.x + depth * norm.x;
        right.y = left.y + depth * norm.y;

        GameObject building = Instantiate(batiment, new Vector3((-y *0.05f + 5) * 10f, 0.5f * height * 10f, (-x * 0.05f + 5) * 10f), Quaternion.identity);
        building.transform.localScale = new Vector3(depth * 10f, height * 10f, width * 10f);

        float theta = (left.y <= right.y) ? 180f - Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f : -Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f;

        building.transform.rotation = Quaternion.AngleAxis(theta, Vector3.up);
        building.transform.Translate(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f * 10f, 0, 0);
    }

    void splitRoads()
    {
        List<Tuple<float, LineSegment>> newMajorRoads = new List<Tuple<float, LineSegment>>(); ;

        float a1 = 0, b1 = 0, a2, b2, x = 0;
        Tuple<float, LineSegment> r1 = null;
        Tuple<float, LineSegment> r2 = null;


        Vector2 begin1 = new Vector2(0, 0);
        Vector2 end1 = new Vector2(0, 0);
        Vector2 begin2 = new Vector2(0, 0);
        Vector2 end2 = new Vector2(0, 0);
        while (majorRoads.Count != 0)
        {
            Boolean end = true;
            r1 = majorRoads[0];


            begin1 = (Vector2)r1.Item2.p0;
            end1 = (Vector2)r1.Item2.p1;
            a1 = (end1.y - begin1.y) / (end1.x - begin1.x);
            b1 = begin1.y - a1 * begin1.x;
            majorRoads.RemoveAt(0);
            foreach (Tuple<float, LineSegment> road2 in roads)
            {
                r2 = road2;
                begin2 = (Vector2)road2.Item2.p0;
                end2 = (Vector2)road2.Item2.p1;

                a2 = (end2.y - begin2.y) / (end2.x - begin2.x);
                b2 = begin2.y - a2 * begin2.x;
                if (Math.Abs(a1 - a2) < 0.0001f)
                    continue;
                x = (b2 - b1) / (a1 - a2);
                if ((begin1.x < x && x < end1.x || end1.x < x && x < begin1.x) && (begin2.x < x && x < end2.x || end2.x < x && x < begin2.x))
                {
                    end = false;
                    break;
                }

            }
            if (!end)
            {
                Vector2 bg1 = new Vector2(begin1.x, begin1.y);
                Vector2 e1 = new Vector2(end1.x, end1.y);
                Vector2 bg2 = new Vector2(begin2.x, begin2.y);
                Vector2 e2 = new Vector2(end2.x, end2.y);
                Vector2 middle = new Vector2(x, a1 * x + b1);
                majorRoads.Add(new Tuple<float, LineSegment>(r1.Item1, new LineSegment(begin1, middle)));
                majorRoads.Add(new Tuple<float, LineSegment>(r1.Item1, new LineSegment(middle, e1)));
                roads.Add(new Tuple<float, LineSegment>(r2.Item1, new LineSegment(bg2, middle)));
                roads.Add(new Tuple<float, LineSegment>(r2.Item1, new LineSegment(middle, e2)));
                roads.Remove(r2);
            }
            else
            {
                newMajorRoads.Add(r1);
            }


        }
        majorRoads = newMajorRoads;
    }

    bool isIntersecting(Tuple<float, LineSegment> t1, Tuple<float, LineSegment> t2)
    {
        Vector2 begin1 = (Vector2)t1.Item2.p0;
        Vector2 end1 = (Vector2)t1.Item2.p1;
        Vector2 begin2 = (Vector2)t2.Item2.p0;
        Vector2 end2 = (Vector2)t2.Item2.p1;



        float a1 = (end1.y - begin1.y) / (end1.x - begin1.x);
        float b1 = begin1.y - a1 * begin1.x;
        float a2 = (end2.y - begin2.y) / (end2.x - begin2.x);
        float b2 = begin2.y - a2 * begin2.x;
        if (Math.Abs(a1 - a2) < 0.0001f)
            return false;
        float x = (b2 - b1) / (a1 - a2);
        if ((begin1.x < x && x < end1.x || end1.x < x && x < begin1.x) && (begin2.x < x && x < end2.x || end2.x < x && x < begin2.x))
        {
            return true;
        }

        return false;

    }

    void removeRoadsOnWater()
    {
        List<Tuple<float, LineSegment>> r = new List<Tuple<float, LineSegment>>();

        foreach(Tuple<float, LineSegment> road in roads)
        {
            bool isValid = true;
            foreach (Tuple<float, LineSegment> w in waterSegments){
                isValid = !isIntersecting(road, w);
                if (!isValid)
                {
                    break;
                }
            }
            if (isValid)
            {
                r.Add(road);
            }
        }
        roads = r;
    }

}
