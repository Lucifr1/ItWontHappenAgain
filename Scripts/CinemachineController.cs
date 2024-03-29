using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    // Cameras
    [SerializeField] private GameObject cameraFolder;
    [SerializeField] private GameObject mainCameraObject;
    [SerializeField] private GameObject followCameraObject;
    [SerializeField] private GameObject kitchenTransitionCameraObject;
    [SerializeField] private GameObject livingRoomTransitionCameraObject;
    [SerializeField] private GameObject doorTransitionCameraObject;
    private CinemachineVirtualCamera mainCamera;
    private CinemachineVirtualCamera followCamera;
    private CinemachineVirtualCamera kitchenTransitionCamera;
    private CinemachineVirtualCamera livingRoomTransitionCamera;
    private CinemachineVirtualCamera doorTransitionCamera;
    private CinemachineVirtualCamera currentTransitionCamera;
    private GameObject playerCameraObject;
    private Camera playerCamera;

    // trigger box
    public String currentlyInThisTriggerBox;
    private String currentTriggerBoxTag;
    public bool hasFinishedTransitionBlending;
    private int skipNextOnTriggerExit;
    private List<String> triggerBoxList;

    void Start()
    {
        playerCameraObject = transform.GetChild(0).gameObject;
        playerCamera = playerCameraObject.GetComponent<Camera>();
        mainCamera = mainCameraObject.GetComponent<CinemachineVirtualCamera>();
        followCamera = followCameraObject.GetComponent<CinemachineVirtualCamera>();
        kitchenTransitionCamera = kitchenTransitionCameraObject.GetComponent<CinemachineVirtualCamera>();
        livingRoomTransitionCamera = livingRoomTransitionCameraObject.GetComponent<CinemachineVirtualCamera>();
        doorTransitionCamera = doorTransitionCameraObject.GetComponent<CinemachineVirtualCamera>();
        triggerBoxList = new List<string>();
        hasFinishedTransitionBlending = true;
        currentTriggerBoxTag = "Nein";
    }

    public void SetPathingCameraSetup(int currentCutscene)
    {
        // set currentCutscene and right trigger box
        switch (currentCutscene)
        {
            case 0:
            {
                currentTriggerBoxTag = "KücheTrigger";
                currentTransitionCamera = kitchenTransitionCamera;
            }
                break;
            case 1:
            {
                currentTriggerBoxTag = "WohnzimmerTrigger";
                currentTransitionCamera = livingRoomTransitionCamera;
            }
                break;
            case 2:
            {
                currentTriggerBoxTag = "WohnzimmerTrigger";
                currentTransitionCamera = livingRoomTransitionCamera;
            }
                break;
            default:
            {
                Debug.Log("Something went wrong setting the right cutscene trigger box");
                currentTriggerBoxTag = "Nein";
            }
                break;
        }

        // set mainCamera position to player position
        mainCamera.transform.position = transform.position;
        mainCamera.transform.forward = playerCameraObject.transform.forward;
        cameraFolder.SetActive(true);
        mainCamera.Priority = 0;
        followCamera.Priority = 1;
        StartCoroutine(DelayBlending());
    }

    // start blending if player is in the correct room
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("WohnzimmerTrigger") || other.gameObject.tag == ("SchlafzimmerTrigger") ||
            other.gameObject.tag == ("BadezimmerTrigger") || other.gameObject.tag == ("ArbeitszimmerTrigger") ||
            other.gameObject.tag == ("KücheTrigger"))
        {
            triggerBoxList.Add(other.gameObject.tag);
        }

        StartCoroutine(DelayBlending());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == ("WohnzimmerTrigger") || other.gameObject.tag == ("SchlafzimmerTrigger") ||
            other.gameObject.tag == ("BadezimmerTrigger") || other.gameObject.tag == ("ArbeitszimmerTrigger") ||
            other.gameObject.tag == ("KücheTrigger"))
        {
            triggerBoxList.Remove(other.gameObject.tag);
        }

        UpdateCurrentTriggerBox();
    }

    private void UpdateCurrentTriggerBox()
    {
        if (triggerBoxList.Count > 0)
        {
            currentlyInThisTriggerBox = triggerBoxList[triggerBoxList.Count - 1];
        }
        else
        {
            currentlyInThisTriggerBox = "";
        }
    }

    private IEnumerator DelayBlending()
    {
        UpdateCurrentTriggerBox();
        if (currentTriggerBoxTag == currentlyInThisTriggerBox)
        {
            yield return new WaitForSeconds(1f);
            followCamera.Priority = 0;
            currentTransitionCamera.Priority = 1;
            StartCoroutine(FinishedTransitionBlending());
        }
    }

    private IEnumerator FinishedTransitionBlending()
    {
        yield return new WaitForSeconds(2f);
        hasFinishedTransitionBlending = true;
    }

    public void EndTransition()
    {
        mainCamera.transform.position = transform.position;
        mainCamera.transform.forward = playerCameraObject.transform.forward;
        cameraFolder.SetActive(true);
        mainCamera.Priority = 0;
        doorTransitionCamera.Priority = 1;
    }

    public void ResetAllCameraSettings()
    {
        cameraFolder.SetActive(false);
        currentTriggerBoxTag = "Nein";
        playerCamera.fieldOfView = 60f;
        playerCamera.nearClipPlane = 0.18f;
        playerCameraObject.transform.localPosition = new Vector3(0, 1.5f, 0.2f);
        playerCameraObject.transform.eulerAngles = new Vector3(0, 0, 0);
        mainCamera.Priority = 1;
        livingRoomTransitionCamera.Priority = 0;
        kitchenTransitionCamera.Priority = 0;
    }
}