using UnityEngine;

public class Basket : MonoBehaviour
{
    public GameObject basketItemPrefab; // Drag the prefab here
    private Transform playerSnappingPoint; // Reference to the player's snapping point (found by tag)

    private bool isPlayerInTrigger = false; // Track whether the player is in the trigger zone
    private bool hasPickedUpItem = false; // Prevent multiple interactions until player leaves trigger
    private InteractUI interactUIScript;

    private void Start()
    {
        // Find the player object by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerSnappingPoint = playerObject.transform.Find("SnappingPoint");
            interactUIScript = playerObject.GetComponent<InteractUI>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInteraction player = other.GetComponent<PlayerInteraction>();

        if (player != null && player.currentItem == null)
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInTrigger = false;
        hasPickedUpItem = false;

        if (interactUIScript != null)
        {
            interactUIScript.ShowInteractUI = false;
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && !hasPickedUpItem)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            PlayerInteraction playerInteraction = playerObject.GetComponent<PlayerInteraction>();

            if (playerInteraction != null && playerInteraction.currentItem == null)
            {
                if (interactUIScript != null)
                {
                    interactUIScript.ShowInteractUI = true;
                }

                // Handle interaction
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (playerSnappingPoint != null)
                    {
                        GameObject itemInstance = Instantiate(basketItemPrefab, playerSnappingPoint.position, playerSnappingPoint.rotation);
                        Item itemInstanceScript = itemInstance.GetComponent<Item>();

                        itemInstanceScript.SnapToPoint(playerSnappingPoint);
                        playerInteraction.currentItem = itemInstanceScript;

                        hasPickedUpItem = true;
                    }
                }
            }
            else if (interactUIScript != null)
            {
                interactUIScript.ShowInteractUI = false;
            }
        }
    }
}
