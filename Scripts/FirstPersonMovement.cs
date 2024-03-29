using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField]private float speed = 12f;
    private Vector3 MovementNextFrame;
    
    // Animation
    private Animator animator;
    private PlayerInteractions playerInteractionsScript;
    [SerializeField] private GameObject rightHand;

    private void Start()
    {
        animator = transform.GetChild(1).GetComponent<Animator>();
        playerInteractionsScript = GetComponent<PlayerInteractions>();
    }

    private void Update()
    {
        MovementNextFrame = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }


    private void FixedUpdate()
    {
        transform.Translate((MovementNextFrame) * Time.deltaTime * speed);
        if (MovementNextFrame != new Vector3(0f, 0f, 0f))
        {
            animator.speed = 1;
            if (playerInteractionsScript.hasObjectInHand)
            {
                animator.SetBool("isWalking", false);
                if (rightHand.transform.GetChild(5).GetComponent<Interactable>().name == "Kleiderstapel")
                {
                    animator.SetBool("isWalkingHoldingBigObject", true);
                }
                else
                {
                    animator.SetBool("isWalkingHoldingObject", true);
                }
            }
            else
            {
                animator.SetBool("isWalkingHoldingBigObject", false);
                animator.SetBool("isWalkingHoldingObject", false);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            if (playerInteractionsScript.hasObjectInHand)
            {
                animator.speed = 0;
            }
            else
            {
                animator.speed = 1;
            }
            animator.SetBool("isWalking", false);
        }
    }
}
