using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private bool isPlayerInTrigger = false; // Track if the player is inside the trashcan's trigger
    private InteractUI interactUIScript;

    private void Start()
    {
        // Find the Interact UI script on the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            interactUIScript = playerObject.GetComponent<InteractUI>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the trigger flag when the player exits the zone
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (interactUIScript != null)
            {
                interactUIScript.ShowInteractUI = false;
            }
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger)
        {
            // Get the PlayerInteraction script
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            PlayerInteraction playerInteraction = playerObject.GetComponent<PlayerInteraction>();

            // Show UI only if the player is holding an item
            if (playerInteraction != null && playerInteraction.currentItem != null)
            {
                if (interactUIScript != null)
                {
                    interactUIScript.ShowInteractUI = true;
                }

                // Check for interaction
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Destroy the currently held item
                    Destroy(playerInteraction.currentItem.gameObject);

                    // Reset the player's current item and holding state
                    playerInteraction.currentItem = null;
                    playerInteraction.isHolding = false;

                    Debug.Log("Item discarded into the trashcan.");
                }
            }
            else if (interactUIScript != null)
            {
                interactUIScript.ShowInteractUI = false;
            }
        }
    }
}
