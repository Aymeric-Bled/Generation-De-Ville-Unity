using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoads : MonoBehaviour
{
    public GameObject route;
    // Start is called before the first frame update
    void Start()
    {
        string[] lines = System.IO.File.ReadAllLines(@".\Python\roads1.txt");
        foreach (string line in lines)
        {
            string[] points = line.Split('\t');
			Vector2 left = new Vector2(float.Parse(points[0]), float.Parse(points[1]));
			Vector2 right = new Vector2(float.Parse(points[2]), float.Parse(points[3]));
			Vector3 vector = new Vector3((-left.y) * 0.05f + 5, 0, (-left.x) * 0.05f + 5);
			GameObject road = Instantiate(route, vector, Quaternion.identity);
			road.transform.localScale = new Vector3(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.05f, 0.03f, 0.1f);

			float theta = (left.y <= right.y) ? 180f - Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f : -Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f;

			/*
			road.transform.rotation = Quaternion.AngleAxis(-Mathf.Atan(((float)right.x - (float)left.x) / ((float)right.y - (float)left.y)) / Mathf.PI * 180f, Vector3.up);

			if (right.y < left.y)
				road.transform.Translate(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f, 0, 0);
			else
				road.transform.Translate(- Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f, 0, 0);
			*/


			road.transform.rotation = Quaternion.AngleAxis(theta, Vector3.up);
			//road.transform.rotation = new Vector3((-left.y) * 0.05f + 5, 0, (-left.x) * 0.05f + 5, 0, theta, 0);

			road.transform.Translate(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) * 0.5f * 0.05f, 0, 0);
			//else
			//road.transform.localScale = new Vector3( - Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2) + Mathf.Pow(right.y - left.y, 2)) / 10, 0.01f, 0.01f);

		}

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
