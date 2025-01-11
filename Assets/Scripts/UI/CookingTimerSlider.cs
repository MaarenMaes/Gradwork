using UnityEngine;
using UnityEngine.UI;

public class CookingTimerSlider : MonoBehaviour
{
    [SerializeField] private Slider cookingSlider; // Reference to the Slider UI component
    [SerializeField] private Canvas canvas; // The canvas containing the slider
    [SerializeField] private Stove stoveScript; // Reference to the Stove script
    [SerializeField] private Camera mainCamera; // Reference to the camera (usually the player's camera)

    void Start()
    {
        // Ensure the slider is always active
        if (cookingSlider != null)
        {
            cookingSlider.gameObject.SetActive(true); // Ensure the slider is visible
        }

        // Ensure the canvas is inactive initially
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false); // Hide the canvas by default
        }

        // If no camera is assigned, try to get the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        SliderBehavior();
        // Apply billboarding effect: make the canvas face the camera
        Billboarding();
    }

    void SliderBehavior()
    {
        // Ensure the stoveScript is assigned
        if (stoveScript != null)
        {
            // If the stove is currently cooking (itemCookTimer > 0)
            if (stoveScript.itemCookTimer > 0.1f)
            {
                // Show the canvas if it's not already active
                if (!canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(true); // Show the canvas
                }

                // Update the slider's value based on the cooking progress
                cookingSlider.value = (stoveScript.cookingTime - stoveScript.itemCookTimer) / stoveScript.cookingTime;
            }
            else
            {
                // If the cooking timer has finished, hide the canvas
                if (canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(false); // Hide the canvas
                }
            }
        }
    }

    void Billboarding()
    {
        if (mainCamera != null)
        {
            // Make the canvas face the camera
            canvas.transform.LookAt(mainCamera.transform);
            // Lock the rotation to only affect the y-axis
            canvas.transform.rotation = Quaternion.Euler(0, canvas.transform.rotation.eulerAngles.y, 0);
        }
    }

    void DebugLog()
    {
        // Debug logs to check values
        Debug.Log(stoveScript.itemCookTimer);
        Debug.Log(stoveScript.cookingTime);
        Debug.Log(cookingSlider.value);
    }
}
