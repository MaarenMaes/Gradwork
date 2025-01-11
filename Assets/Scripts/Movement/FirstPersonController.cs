using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6.0f;           // Walking speed
    public float jumpHeight = 1.5f;      // Jump height in units
    public float gravity = -9.81f;       // Gravity applied to the character

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 300.0f;
    public Transform cameraTransform;    // Reference to the camera

    [Header("Ground Check Settings")]
    public float groundDistance = 0.4f;  // How far the ray will check for the ground
    public LayerMask groundMask;         // Layer mask to specify what objects are considered as "ground"

    private CharacterController controller;
    private Vector3 velocity;            // Used for vertical movement (jumping/gravity)
    private float xRotation = 0f;        // Camera vertical rotation

    void Start()
    {
        // Get the CharacterController component
        controller = GetComponent<CharacterController>();

        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        // Get input for movement (WASD or Arrow keys)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Combine inputs into a movement vector relative to the player's forward direction
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Apply movement
        controller.Move(move * speed * Time.deltaTime);

        // Check if the player is grounded using raycast
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);

        // Visualize the raycast in the scene view
        Debug.DrawRay(transform.position, Vector3.down * groundDistance, isGrounded ? Color.green : Color.red);

        // If grounded, apply small downward force to ensure the player sticks to the ground
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // A small negative velocity to keep the player grounded
        }

        // Jumping (only if grounded)
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate jump velocity
            Debug.Log("Jump");
        }

        // Apply gravity (always, even if grounded)
        velocity.y += gravity * Time.deltaTime;

        // Move the character based on vertical velocity (gravity/jumping)
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera vertically (clamped to avoid flipping)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);
    }
}
