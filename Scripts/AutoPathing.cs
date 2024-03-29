using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AutoPathing : MonoBehaviour
{
    //[SerializeField] private Vector3 cutsceneDestination;
    [SerializeField] private Vector3 firstCutsceneDestination;
    [SerializeField] private Vector3 secondCutsceneDestination;
    [SerializeField] private Vector3 thirdCutsceneDestination;

    private NavMeshAgent navMeshAgent;
    public bool hasReachedDestination;

    // Animations
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        hasReachedDestination = true;
        animator = transform.GetChild(1).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ResetDestination();
    }

    public void GetToCurrentCutscenePoint(int currentCutscene)
    {
        navMeshAgent.enabled = true;
        animator.SetBool("isWalking", true);
        switch (currentCutscene)
        {
            case 0:
            {
                navMeshAgent.destination = firstCutsceneDestination;
            }
                break;
            case 1:
            {
                navMeshAgent.destination = secondCutsceneDestination;
            }
                break;
            case 2:
            {
                navMeshAgent.destination = thirdCutsceneDestination;
            }
                break;
            default:
            {
                navMeshAgent.destination = navMeshAgent.transform.position;
            }
                break;
        }
    }


    private void ResetDestination()
    {
        if (CheckIfNavMeshAgentHasReachedDestination())
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
        }
    }


    private bool CheckIfNavMeshAgentHasReachedDestination()
    {
        if (navMeshAgent.transform.position.x == navMeshAgent.destination.x &&
            navMeshAgent.transform.position.z == navMeshAgent.destination.z)
        {
            animator.SetBool("isWalking", false);
            hasReachedDestination = true;
            return true;
        }

        return false;
    }
}