using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Interactable : MonoBehaviour
{
    // Interactable
    private GameObject player;

    public string name;
    public bool isInFinalPosition;

    private LoopController loopController;

    private GameObject pickUpInteractableEmptyGameObjects;
    [SerializeField] private Vector3 startPosition;
    private Vector3 startRotation;

    [SerializeField] private GameObject Ablageort;
    private GameObject lighting;
    private Volume lightingVolume;
    private Fog fog;
    
    [SerializeField] private GameObject ItemPlaceholder;

    // Deny interaction
    [SerializeField] public string interactableTriggerRoom;
    private GameObject guyController;
    private AutoPathingGuy autoPathingGuy;
    public bool impossibleToInteract;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lighting = GameObject.FindGameObjectWithTag("Lighting");
        lightingVolume = lighting.GetComponent<Volume>();
        lightingVolume.profile.TryGet(out fog);
        loopController = GameObject.Find("LoopController").GetComponent<LoopController>();
        guyController = GameObject.FindGameObjectWithTag("GuyController");
        autoPathingGuy = guyController.GetComponent<AutoPathingGuy>();
        pickUpInteractableEmptyGameObjects = GameObject.Find("PICKUPINTERACTABLES").gameObject;
        startPosition = transform.position;
        startRotation = transform.localEulerAngles;
        impossibleToInteract = false;
        
    }

    private void Update()
    {
        if (name == "Weinflasche" || name == "Fernbedienung")
        {
            if (autoPathingGuy.hasReachedCouch)
            {
                impossibleToInteract = true;
                autoPathingGuy.hasReachedCouch = false;
            }
        }
    }


    public void AddObjectToPlayer()
    {
        if (isInFinalPosition)
        {
            return;
        }

        transform.GetChild(3).gameObject.SetActive(false);
        gameObject.transform.SetParent(ItemPlaceholder.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        player.GetComponent<PlayerInteractions>().hasObjectInHand = true;
        fog.enabled.value = true;
        Ablageort.transform.GetChild(1).gameObject.SetActive(true);
        Ablageort.transform.GetChild(2).gameObject.SetActive(true);

        
        //FIX POSITION
        if (name == "Kleiderstapel")
        {
            transform.localPosition = new Vector3(0.06f,0.07f,0.23f);
            transform.localRotation = Quaternion.Euler(3, 0, 95f);
            transform.localScale = new Vector3(0.56f, 0.56f, 0.56f);
        }
        
        if (name == "Weinflasche")
        {
            transform.localPosition = new Vector3(0.3f,0.05f,0.03f);
            transform.localRotation = Quaternion.Euler(0, 0, 90f);
        }

        if (name == "Fernbedienung")
        {
            transform.localPosition = new Vector3(0,0.067f,0.035f);
            transform.localRotation = Quaternion.Euler(324.23f,71.1f,87.9f);
            transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        }
    }

    public void AddObjectToPlace(GameObject place)
    {
        gameObject.transform.SetParent(place.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        isInFinalPosition = true;
        player.GetComponent<PlayerInteractions>().hasObjectInHand = false;
        fog.enabled.value = false;
        Ablageort.transform.GetChild(1).gameObject.SetActive(false);
        Ablageort.transform.GetChild(2).gameObject.SetActive(false);
    }

    public void ResetObjectToPickUpPosition()
    {
        Debug.Log("Resetting objects");
        gameObject.transform.SetParent(pickUpInteractableEmptyGameObjects.transform);
        transform.localPosition = startPosition;
        transform.localEulerAngles = startRotation;
        player.GetComponent<PlayerInteractions>().hasObjectInHand = false;
        isInFinalPosition = false;
        impossibleToInteract = false;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        transform.GetChild(3).gameObject.SetActive(true);
    }
}