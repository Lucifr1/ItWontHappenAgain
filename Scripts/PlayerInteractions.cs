using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.AI;

public class PlayerInteractions : MonoBehaviour
{
    public bool hasObjectInHand;
    private LoopController loopController;

    // Raycast
    private GameObject camera;
    private RaycastHit hit;
    public static float raycastMaxrange = 2f;
    private GameObject hitObject;
    [SerializeField] private GameObject rightHand;

    // World UI
    private GameObject objectWorldCanvas;

    // Deny interaction and animation by man
    [SerializeField] private GameObject manGameObject;
    private AutoPathingGuy autoPathingGuy;
    private Animator animatorMan;
    public bool currentlyTriggered;

    // Memory cutscenes
    public GameObject erinnerungsObjekte;

    // Door
    private bool endingStarted;

    // Voicelines and general feedback
    private VoicelineController voicelineController;
    private Subtitles subtitles;
    private bool justPlayedInteractable;

    // Animation
    private Animator animator;


    void Start()
    {
        camera = transform.GetChild(0).gameObject;
        autoPathingGuy = manGameObject.GetComponent<AutoPathingGuy>();
        animatorMan = manGameObject.transform.GetChild(0).GetComponent<Animator>();
        loopController = GameObject.FindGameObjectWithTag("LoopController").GetComponent<LoopController>();
        endingStarted = false;
        subtitles = GameObject.FindGameObjectWithTag("Subtitle").GetComponent<Subtitles>();
        voicelineController = transform.GetChild(2).GetComponent<VoicelineController>();
        currentlyTriggered = false;
        justPlayedInteractable = false;
        animator = transform.GetChild(1).GetComponent<Animator>();
    }

    void Update()
    {
        Debug.DrawRay(camera.transform.position, camera.transform.forward * hit.distance, Color.green);
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, raycastMaxrange))
        {
            hitObject = hit.collider.gameObject;
            if (hit.collider.gameObject.CompareTag("Interactable") &&
                hit.collider.gameObject.GetComponent<Interactable>().enabled && !loopController.playerAutoWalkingStarts)
            {
                PickUpInteractable();
            }

            if (hit.collider.gameObject.CompareTag("Ablegeplatz") && hasObjectInHand)
            {
                if (Equals(rightHand.transform.GetChild(5).GetComponent<Interactable>().name,
                    hitObject.GetComponent<ItemPlace>().correctItemToGoHere))
                {
                    PlaceInteractable();
                }
            }

            if (hit.collider.gameObject.CompareTag("Door") && !endingStarted)
            {
                EndDoor();
            }

            if (hit.collider.gameObject.CompareTag("Erinnerungsobjekt"))
            {
                PlayErinnerungsobjektSchattenspiel();
            }
        }
        else if (objectWorldCanvas != null)
        {
            objectWorldCanvas.SetActive(false);
            justPlayedInteractable = false;
        }
    }

    private void PlayErinnerungsobjektSchattenspiel()
    {
        if (hit.collider.gameObject.GetComponent<Erinnerungsobjekt>().erinnerungsObjektAngeschaut)
        {
            return;
        }

        for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
        {
            if (erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().isPlaying)
            {
                return;
            }
        }

        objectWorldCanvas = hitObject.transform.GetChild(1).gameObject;
        objectWorldCanvas.SetActive(true);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!hit.collider.gameObject.GetComponent<Erinnerungsobjekt>().enabled)
            {
                // Play voiceline deny Erinnerungsobjekt
                voicelineController.SetCurrentVoiceline("DenyErinnerungEr");
                subtitles.StartSubtitles("Du bist noch nicht bereit, dich dieser Erinnerung zu stellen.", 5f);
            }
            else
            {
                objectWorldCanvas.SetActive(false);
                hit.collider.gameObject.GetComponent<Erinnerungsobjekt>().startVideoSequence();
            }
        }
    }

    private void EndDoor()
    {
        objectWorldCanvas = hitObject.transform.GetChild(1).gameObject;
        objectWorldCanvas.SetActive(true);
        if (Input.GetKeyDown(KeyCode.F))
        {
            endingStarted = true;
            objectWorldCanvas.SetActive(false);
            loopController.EndGame();
        }
    }

    private void PickUpInteractable()
    {
        if (objectWorldCanvas != null)
        {
            objectWorldCanvas.SetActive(false);
        }

        if (!hasObjectInHand && !hitObject.GetComponent<Interactable>().isInFinalPosition)
        {
            // check if man is not in the same room
            if (hitObject.GetComponent<Interactable>().interactableTriggerRoom == autoPathingGuy.currentRoomManIsIn)
            {
                // Voiceline triggers when you look at the interactable
                if (!justPlayedInteractable)
                {
                    justPlayedInteractable = true;
                    voicelineController.SetCurrentVoiceline("DenyInteractionSie");
                }

                objectWorldCanvas = hitObject.transform.GetChild(1).gameObject;
                objectWorldCanvas.SetActive(false);
                objectWorldCanvas = hitObject.transform.GetChild(2).gameObject;
                objectWorldCanvas.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F) && !currentlyTriggered)
                {
                    currentlyTriggered = true;
                    // Play randomized deny voiceline
                    voicelineController.SetCurrentVoiceline("DenyInteraktionEr");
                    if (!autoPathingGuy.hasReachedDestination)
                    {
                        StartCoroutine(ManDenyAnimationDelayWalking());
                    }
                    else
                    {
                        StartCoroutine(ManDenyAnimationDelayStanding());
                    }
                }
            }
            else
            {
                objectWorldCanvas = hitObject.transform.GetChild(2).gameObject;
                objectWorldCanvas.SetActive(false);
                objectWorldCanvas = hitObject.transform.GetChild(1).gameObject;
                objectWorldCanvas.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    // Pick up animation
                    /*
                    if (hit.collider.gameObject.transform.GetChild(0).CompareTag("LaundryBasket"))
                    {
                        Debug.Log("Pick up laundry basket");
                        animator.SetTrigger("laundryBasketPickUp");
                    }
                    else
                    {
                        animator.SetTrigger("pickUp");
                    }*/
                    if (hitObject.GetComponent<Interactable>().name == "Kleiderstapel")
                    {
                        animator.SetBool("isWalkingHoldingBigObject", true);
                    }
                    else
                    {
                        animator.SetBool("isWalkingHoldingObject", true);
                    }

                    objectWorldCanvas.SetActive(false);
                    hitObject.GetComponent<BoxCollider>().enabled = false;
                    hitObject.GetComponent<Interactable>().AddObjectToPlayer();
                }
            }
        }
    }

    IEnumerator ManDenyAnimationDelayStanding()
    {
        animatorMan.SetTrigger("deny");
        manGameObject.transform.GetChild(0).transform.LookAt(transform);
        yield return new WaitForSeconds(5f);
        manGameObject.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, -90, 0);
        currentlyTriggered = false;
    }

    IEnumerator ManDenyAnimationDelayWalking()
    {
        manGameObject.GetComponent<NavMeshAgent>().isStopped = true;
        animatorMan.SetTrigger("deny");
        manGameObject.transform.GetChild(0).transform.LookAt(transform);
        yield return new WaitForSeconds(2.5f);
        manGameObject.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, -90, 0);
        manGameObject.GetComponent<NavMeshAgent>().isStopped = false;
        currentlyTriggered = false;
    }

    private void PlaceInteractable()
    {
        objectWorldCanvas = hitObject.transform.GetChild(0).gameObject;
        objectWorldCanvas.SetActive(true);
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Place animation
            /*
            if (hitObject.GetComponent<ItemPlace>().correctItemToGoHere == "Kleiderstapel")
            {
                Debug.Log("Place laundry basket");
                animator.SetTrigger("laundryBasketPlace");
            }
            else
            {
                animator.SetTrigger("place");
            }*/
            if (rightHand.transform.GetChild(5).GetComponent<Interactable>().name == "Kleiderstapel")
            {
                animator.SetBool("isWalkingHoldingBigObject", false);
            }
            else
            {
                animator.SetBool("isWalkingHoldingObject", false);
            }

            rightHand.transform.GetChild(5).GetComponent<Interactable>().AddObjectToPlace(hitObject);
            objectWorldCanvas.SetActive(false);
        }
    }
}