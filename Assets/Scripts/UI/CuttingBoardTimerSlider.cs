using UnityEngine;
using UnityEngine.UI;

public class CuttingBoardTimerSlider : MonoBehaviour
{
    [SerializeField] private Slider cuttingSlider; // Reference to the Slider UI component
    [SerializeField] private Canvas canvas; // The canvas containing the slider
    [SerializeField] private CuttingBoard cuttingBoardScript; // Reference to the CuttingBoard script
    [SerializeField] private Camera mainCamera; // Reference to the camera (usually the player's camera)

    void Start()
    {
        // Ensure the slider is inactive initially
        if (cuttingSlider != null)
        {
            cuttingSlider.gameObject.SetActive(true); // Hide the slider by default
        }

        // If no camera is assigned, try to get the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        UpdateSliderBehavior();
        ApplyBillboardingEffect();
    }

    private void UpdateSliderBehavior()
    {
        // Ensure the cuttingBoardScript is assigned
        if (cuttingBoardScript != null)
        {
            // Check if there's an active cutting process
            if (cuttingBoardScript.itemCutTimer > 0)
            {
                // Show the canvas if cutting is in progress
                if (!canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(true);
                }

                // Update the slider's value based on the cutting progress (0 to 1)
                cuttingSlider.value = (cuttingBoardScript.cuttingTime - cuttingBoardScript.itemCutTimer) / cuttingBoardScript.cuttingTime;
            }
            else
            {
                // Hide the canvas when cutting is not in progress
                if (canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ApplyBillboardingEffect()
    {
        if (mainCamera != null)
        {
            // Make the canvas face the camera
            canvas.transform.LookAt(mainCamera.transform);
            canvas.transform.rotation = Quaternion.Euler(0, canvas.transform.rotation.eulerAngles.y, 0); // Lock rotation to y-axis
        }
    }
}
