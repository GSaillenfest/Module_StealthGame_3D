using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrolling : MonoBehaviour
{

    [SerializeField] Transform[] patrolPoints;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform eyes;
    [SerializeField] Transform detectionPoint;
    [SerializeField] GameObject alertMark;
    [SerializeField] GameObject gameOver;



    int i = 0;
    float t;
    bool isWaiting;
    public bool alert;
    float detectionTime;
    Vector3 destinationMemory;


    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(patrolPoints[i].position);
        alertMark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance <= 0.1f)
        {
            isWaiting = true;
            Wait();
            if (isWaiting == false)
            {
                if (i < patrolPoints.Length - 1) i++;
                else i = 0;
                agent.SetDestination(patrolPoints[i].position);
                t = Random.Range(0.5f, 3f);
            }
        }

        if (alert)
        {
            Detection();
        }

    }

    void Wait()
    {
        t += Time.deltaTime;
        if (t > 3f)
        {
            isWaiting = false;
            t = 0;
        }
    }

    void Detection()
    {
        Debug.Log("PlayerInEyeSight");
        alertMark.SetActive(true);
        Ray ray = new Ray(eyes.position, detectionPoint.position - eyes.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) & hit.transform.CompareTag("Player"))
        {
            Debug.Log("PlayerDetected : " + detectionTime);
            alertMark.GetComponent<Renderer>().material.color = Color.yellow;
            detectionTime += Time.deltaTime;
            destinationMemory = agent.destination;
            agent.SetDestination(transform.position);
        }
        else
        {
            detectionTime -= Time.deltaTime * 0.5f;

        }

        if (detectionTime > 5f)
        {
            alertMark.GetComponent<Renderer>().material.color = Color.red;
            GameOver();
        }
    }

    public void StopDetection()
    {
        detectionTime = 0;
        alert = false;
        alertMark.SetActive(false);
        agent.SetDestination(destinationMemory);
        alertMark.GetComponent<Renderer>().material.color = Color.white;
    }

    private void GameOver()
    {
        Time.timeScale = 0.2f;
        gameOver.SetActive(true);
    }


}
