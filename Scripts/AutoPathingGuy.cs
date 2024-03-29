using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AutoPathingGuy : MonoBehaviour
{
    [SerializeField] private GameObject player;

    // Autopathing
    public List<Vector3> destinations;
    public List<float> waitingTimes;
    public NavMeshAgent navMeshAgent;
    private int numberDestinationsReachedInOneLoop;
    public bool hasReachedDestination;
    public bool hasReachedCouch;
    public bool loopHasStarted;
    private Rigidbody rb;
    public int loopNumber;
    private int currentLoop;

    // Autorotation
    public List<Vector3> onSpotRotation;
    private bool hasReachedRotation;
    [SerializeField] private float rotationSpeed;

    // Sitting animation
    private Vector3 goalPosition;
    private Quaternion goalRotation;

    // Deny interaction -> in the same room
    private List<String> triggerBoxListGuy;
    public string currentRoomManIsIn;
    private PlayerInteractions playerInteractionsScript;

    // Animations
    private Animator animator;

    [SerializeField] private GameObject sitzTriggerBox;

    void Start()
    {
        numberDestinationsReachedInOneLoop = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerInteractionsScript = player.GetComponent<PlayerInteractions>();
        triggerBoxListGuy = new List<string>();

        // position check
        currentRoomManIsIn = "";

        animator = transform.GetChild(0).GetComponent<Animator>();

        loopHasStarted = false;
        hasReachedRotation = true;
        hasReachedCouch = false;
        loopNumber = 1;
    }

    void Update()
    {
        if (!hasReachedDestination)
        {
            CheckIfNavMeshAgentHasReachedDestination();
        }
    }

    public void StartWalkCycle()
    {
        Debug.Log("Starting walk cycle");
        hasReachedDestination = false;
        navMeshAgent.destination = destinations[0];
        loopHasStarted = true;
        animator.SetBool("resetSitting", false);
        animator.SetBool("isWalking", true);
    }

    private void ResetDestination()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        animator.SetBool("isWalking", false);
    }

    IEnumerator WaitAtLocation()
    {
        currentLoop = loopNumber;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(waitingTimes[numberDestinationsReachedInOneLoop]);
        if (currentLoop != loopNumber)
        {
            yield break;
        }

        Debug.Log(navMeshAgent.destination);
        Debug.Log("Next destination");
        if (playerInteractionsScript.currentlyTriggered)
        {
            yield return new WaitUntil(() => !playerInteractionsScript.currentlyTriggered);
        }

        numberDestinationsReachedInOneLoop++;
        if (numberDestinationsReachedInOneLoop < destinations.Count)
        {
            navMeshAgent.destination = destinations[numberDestinationsReachedInOneLoop];
            Debug.Log(navMeshAgent.destination);
            hasReachedDestination = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            animator.SetBool("isWalking", true);
        }
        else
        {
            // Sofa erreicht
            // Set position and rotation
            hasReachedCouch = true;
            navMeshAgent.enabled = false;
            goalPosition = new Vector3(-4.38199997f, 0.263000011f, 18.2380009f);
            goalRotation = new Quaternion(0, 0.659559846f, 0, -0.751652062f);
            StartCoroutine(GetToCoachSittingPosition(transform.position, goalPosition, 2f));
            StartCoroutine(GetToCoachRotationPosition(transform.rotation, goalRotation, 2f));
            animator.SetTrigger("sitDown");
        }
    }

    IEnumerator GetToCoachSittingPosition(Vector3 startPos, Vector3 endPos, float time)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    IEnumerator GetToCoachRotationPosition(Quaternion startRotation, Quaternion endRotation, float time)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
    }

    private void CheckIfNavMeshAgentHasReachedDestination()
    {
        if (navMeshAgent.transform.position.x == navMeshAgent.destination.x &&
            navMeshAgent.transform.position.z == navMeshAgent.destination.z && loopHasStarted)
        {
            hasReachedDestination = true;
            ResetDestination();
            StartCoroutine(WaitAtLocation());
        }
    }

    private void CheckIfNavMeshAgentHasReachedRotation(int currentPosition)
    {
        if (transform.forward == onSpotRotation[currentPosition])
        {
            Debug.Log("Finished Rotating");
            hasReachedRotation = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("WohnzimmerTrigger") || other.gameObject.tag == ("SchlafzimmerTrigger") ||
            other.gameObject.tag == ("BadezimmerTrigger") || other.gameObject.tag == ("ArbeitszimmerTrigger") ||
            other.gameObject.tag == ("KücheTrigger"))
        {
            triggerBoxListGuy.Add(other.gameObject.tag);
        }

        UpdateCurrentTriggerBox();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == ("WohnzimmerTrigger") || other.gameObject.tag == ("SchlafzimmerTrigger") ||
            other.gameObject.tag == ("BadezimmerTrigger") || other.gameObject.tag == ("ArbeitszimmerTrigger") ||
            other.gameObject.tag == ("KücheTrigger"))
        {
            triggerBoxListGuy.Remove(other.gameObject.tag);
        }

        UpdateCurrentTriggerBox();
    }

    private void UpdateCurrentTriggerBox()
    {
        if (triggerBoxListGuy.Count > 0)
        {
            currentRoomManIsIn = triggerBoxListGuy[triggerBoxListGuy.Count - 1];
        }
        else
        {
            currentRoomManIsIn = "";
        }
    }

    public void ResetGuyBackToStartofLoop()
    {
        Debug.Log("resetting guy to start of loop");
        transform.position = new Vector3(-3, 0, 3);
        animator.SetBool("resetSitting", true);
        navMeshAgent.enabled = true;
        ResetDestination();
        numberDestinationsReachedInOneLoop = 0;
    }
}