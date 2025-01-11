using UnityEngine;

public class NpcUI : MonoBehaviour
{
    // References to the two canvases (set these in the Unity Inspector)
    [SerializeField] private Canvas recipeOneCanvas; // First canvas for Recipe 1
    [SerializeField] private Canvas recipeTwoCanvas; // Second canvas for Recipe 2

    // Boolean variables to determine which recipe to show
    public bool isRecipeOne = false; // Whether recipe one is selected
    public bool isRecipeTwo = false; // Whether recipe two is selected

    // Reference to the main camera (set this in the Unity Inspector or find it in code)
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    // Update is called once per frame
    void Update()
    {
        // Apply billboarding effect
        BillboardCanvas();

        // Check if either recipe flag is true, and show the appropriate canvas
        if (isRecipeOne)
        {
            ShowCanvas(recipeOneCanvas, recipeTwoCanvas);
        }
        else if (isRecipeTwo)
        {
            ShowCanvas(recipeTwoCanvas, recipeOneCanvas);
        }
        else
        {
            HideAllCanvases(); // If neither recipe is selected, hide both
        }
    }

    // Function to apply the billboarding effect to the canvases
    private void BillboardCanvas()
    {
        // Make both canvases face the camera
        if (mainCamera != null)
        {
            if (recipeOneCanvas != null)
            {
                recipeOneCanvas.transform.LookAt(mainCamera.transform);
                recipeOneCanvas.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }

            if (recipeTwoCanvas != null)
            {
                recipeTwoCanvas.transform.LookAt(mainCamera.transform);
                recipeTwoCanvas.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }
    }

    // Function to show one canvas and hide the other
    private void ShowCanvas(Canvas canvasToShow, Canvas canvasToHide)
    {
        // Enable the desired canvas and disable the other one
        if (canvasToShow != null)
        {
            canvasToShow.gameObject.SetActive(true); // Show the canvas
        }

        if (canvasToHide != null)
        {
            canvasToHide.gameObject.SetActive(false); // Hide the other canvas
        }
    }

    // Function to hide both canvases
    private void HideAllCanvases()
    {
        if (recipeOneCanvas != null)
        {
            recipeOneCanvas.gameObject.SetActive(false); // Hide recipe 1 canvas
        }

        if (recipeTwoCanvas != null)
        {
            recipeTwoCanvas.gameObject.SetActive(false); // Hide recipe 2 canvas
        }
    }
}
