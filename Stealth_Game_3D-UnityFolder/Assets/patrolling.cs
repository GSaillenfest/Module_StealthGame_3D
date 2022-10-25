using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class patrolling : MonoBehaviour
{

    [SerializeField] Transform[] patrolPoints;
    [SerializeField] NavMeshAgent agent;
    

    int i = 0;


    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(patrolPoints[i].position);

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(agent.remainingDistance);
        if (agent.remainingDistance <= 0.1f)
        {
            Debug.Log("Arrived");
            if (i < patrolPoints.Length - 1) i++;
            else i = 0;
            agent.SetDestination(patrolPoints[i].position);
        }
    }
}
