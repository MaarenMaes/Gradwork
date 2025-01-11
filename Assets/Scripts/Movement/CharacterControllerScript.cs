using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f; // Speed of the player movement
    public float rotationSpeed = 720f; // Speed of rotation in degrees per second

    [Header("Animation Settings")]
    public Animator animator; // Animator component
    public Avatar avatar; // Avatar for the Animator (optional in this case)

    private CharacterController characterController; // CharacterController component
    private Vector3 movementInput; // Input for movement
    private Quaternion targetRotation; // Desired rotation of the character

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController is missing from the character. Add a CharacterController component.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator is missing from the character. Assign an Animator component.");
        }

        if (avatar != null)
        {
            // Set the Avatar for the Animator (if provided)
            animator.avatar = avatar;
        }
        else
        {
            Debug.LogWarning("No Avatar assigned to Animator.");
        }
    }

    void Update()
    {
        // Get movement input from player (WASD or Arrow Keys)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Normalize movement input to avoid diagonal speed boost
        movementInput = new Vector3(horizontal, 0, vertical).normalized;

        // If there's movement input, update the target rotation
        if (movementInput != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);
        }

        // Apply movement only if there is input
        if (movementInput != Vector3.zero)
        {
            Vector3 movement = movementInput * moveSpeed * Time.deltaTime;
            characterController.Move(movement);

            // Apply rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update Animator parameter for controlling animations
        float speed = movementInput.magnitude * moveSpeed;
        animator.SetFloat("MovementSpeed", speed); // Set the Speed parameter to control animation state
    }

}
