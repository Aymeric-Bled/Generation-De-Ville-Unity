using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Trafic : MonoBehaviour
{

    public NavMeshSurface surface;
    public NavMeshAgent car;

    // Start is called before the first frame update
    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();


        
        //GameObject test_ = Instantiate(test, new Vector3(-5, (float)-0, -5), Quaternion.Euler(0, 0, 90));
        //c = (NavMeshAgent)Instantiate(car, new Vector3((float)3.5,(float)0.5,(float)37.5), Quaternion.Euler(0,0,0));
        //c.destination = new Vector3(-(float)18, (float)0.5, (float)37.5);

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (c.velocity == new Vector3(0,0,0))
        {
            c.destination = new Vector3(-18, (float)0.5, (float)37.5);
        }
        */
        
    }
}
