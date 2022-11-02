using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationSetter : MonoBehaviour
{

    [SerializeField] Animator animator;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = transform.parent.GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("DirectionMagnitude", agent.velocity.magnitude / agent.speed);
        animator.SetFloat("AngleBetweenForwardAndDirection", Vector3.SignedAngle(transform.forward, agent.velocity, Vector3.up));
    }
}
