using UnityEngine;

public class NpcOrder : MonoBehaviour
{
    public bool isRecipe1OnPlate = false; // Tracks if the NPC wants Recipe 1
    public bool isRecipe2OnPlate = false; // Tracks if the NPC wants Recipe 2

    [SerializeField] private bool isPlayerNearby = false; // To check if player is inside the interaction trigger area

    // Reference to the NpcUI script
    public NpcUI npcUI;

    // Trigger detection to check if player is in range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Randomly select a recipe (1 or 2)
        int randomRecipe = Random.Range(1, 3); // Random.Range(1, 3) returns 1 or 2

        // Assign the NPC's order based on the random selection
        SetNpcOrder(randomRecipe);
    }

    // Set NPC's order depending on the random recipe selected
    private void SetNpcOrder(int recipeNumber)
    {
        if (recipeNumber == 1)
        {
            isRecipe1OnPlate = true;
            // Update the NPC UI to show the recipe 1 canvas
            npcUI.isRecipeOne = true;
            npcUI.isRecipeTwo = false;
        }
        else if (recipeNumber == 2)
        {
            isRecipe2OnPlate = true;
            // Update the NPC UI to show the recipe 2 canvas
            npcUI.isRecipeOne = false;
            npcUI.isRecipeTwo = true;
        }
    }

    // Handle interaction with the player
    private void HandlePlayerInteraction(PlayerInteraction player)
    {
        if (!isPlayerNearby || !player.isHolding) return;

        Item playerItem = player.currentItem;

        // Check if the player is holding a plate and if the plate is what the NPC wants
        if (playerItem != null)
        {
            // Check if the NPC wants Recipe 1 or Recipe 2 and handle the interaction
            if (isRecipe1OnPlate && playerItem.IsRecipe1ProductOnPlate)
            {
                // Player has given the correct Recipe 1 on plate
                CompleteOrder();
            }
            else if (isRecipe2OnPlate && playerItem.IsRecipe2ProductOnPlate)
            {
                // Player has given the correct Recipe 2 on plate
                CompleteOrder();
            }
            else
            {
                // Player gave an incorrect dish
                //Debug.Log("This is not the correct recipe.");
            }
        }
    }

    // Complete the order and reset the NPC's expectation
    public void CompleteOrder()
    {
        // Handle order completion (give reward, update UI, etc.)
        //Debug.Log("Order completed!");

        // Reset the NPC's order
        isRecipe1OnPlate = false;
        isRecipe2OnPlate = false;

        // Hide both canvases since the order is complete
        npcUI.isRecipeOne = false;
        npcUI.isRecipeTwo = false;

        // Notify the NPC movement system to continue its path
        NpcMovement npcMovement = GetComponent<NpcMovement>();
        if (npcMovement != null)
        {
            //Debug.Log("Moving NPC to next waypoint...");
            
            npcMovement.SetOrderFilled(true);
            //npcMovement.MoveToNextWaypointAfterOrder();  // Move to the next waypoint
        }
    }
}
