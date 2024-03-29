using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderWallVisibility : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [HideInInspector] public Material borderwall;
    private Color colorCopy;
    private float opacity = 1f;
    private float distance;
    private float minDistance = 20f;
    private float opacityChange = 1.5f;
    private RaycastHit hit;
    private Vector3 rotationWall;
    private int LayerMask = 1 << 6;


    void Start()
    {
        borderwall = GetComponent<MeshRenderer>().material;
        colorCopy = borderwall.color;
        if (transform.rotation.w > 0.5)
        {
            rotationWall = Vector3.right;
        }
        else if (transform.rotation.w > 0)
        {
            rotationWall = Vector3.back;
        }
        else if (transform.rotation.w > -0.6)
        {
            rotationWall = Vector3.forward;
        }
        else
        {
            rotationWall = Vector3.left;
        }
    }

    void FixedUpdate()
    {
        Physics.Raycast(player.transform.GetChild(1).transform.position, rotationWall, out hit, Mathf.Infinity,
            LayerMask);
        distance = hit.distance;
        if (distance > minDistance)
        {
            opacity = (float) (Math.Pow((minDistance / distance), opacityChange)) * 1;
        }

        if (opacity < 0.1)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        colorCopy.a = opacity;
        borderwall.color = colorCopy;
    }
}