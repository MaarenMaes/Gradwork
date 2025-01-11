using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    // Start is called before the first frame update
    public bool ShowInteractUI;

    [SerializeField] private GameObject interactUICanvas;

    // Reference to the main camera
    private Camera mainCamera;

    void Start()
    {
        ShowInteractUI = false;
        interactUICanvas.SetActive(false);

        // Initialize the main camera (camera the player is using)
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        EnableInteractUI();
        FaceCamera();
    }

    void EnableInteractUI()
    {
        if (ShowInteractUI)
        {
            interactUICanvas.SetActive(true);
        }
        else
        {
            interactUICanvas.SetActive(false);
        }
    }

    // Makes the UI always face the camera
    void FaceCamera()
    {
        if (mainCamera != null)
        {
            // Get the direction from the UI to the camera
            Vector3 direction = interactUICanvas.transform.position - mainCamera.transform.position;

            // Set the UI's rotation to face the camera, ignoring the Y-axis rotation to keep it upright
            interactUICanvas.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
