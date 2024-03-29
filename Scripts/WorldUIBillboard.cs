using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIBillboard : MonoBehaviour
{
    public Transform camera;

    // LateUpdate makes that the camera has already moved
    void LateUpdate()
    {
        transform.LookAt(transform.position + camera.forward);
    }
}
