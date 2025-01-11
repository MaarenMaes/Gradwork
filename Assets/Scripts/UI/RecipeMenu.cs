using UnityEngine;

public class RecipeMenu : MonoBehaviour
{
    // Reference to the Canvas (you can set this in the Unity Inspector)
    public Canvas pauseMenuCanvas; // The Canvas that holds your pause menu sprite
    private bool isPaused = false; // To track if the game is paused

    // Update is called once per frame
    private void Awake()
    {
        TogglePause();
        pauseMenuCanvas.gameObject.SetActive(true);
    }
    void Update()
    {
        // Check if the player presses the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Function to toggle pause and resume
    void TogglePause()
    {
        isPaused = !isPaused;

        // If the game is paused, show the pause menu and pause time
        if (isPaused)
        {
            pauseMenuCanvas.gameObject.SetActive(true); // Show the pause menu Canvas
            //Time.timeScale = 0f; // Pause the game (time stops)
        }
        else
        {
            pauseMenuCanvas.gameObject.SetActive(false); // Hide the pause menu Canvas
            //Time.timeScale = 1f; // Resume the game (time resumes)
        }
    }
}
