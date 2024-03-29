using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]private float mouseSensitivity = 2f;
    [SerializeField] private float smoothing = 1.5f;
    [SerializeField] private Transform playerBody;

    private Vector2 velocity;
    private Vector2 frameVelocity;
    
    // Locks the mouse cursor
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * mouseSensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -60, 90);

        // Rotate camera up-down and controller left-right from velocity
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        playerBody.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
