using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material[] materials;
    private PlayerInteraction playerInteraction; // Reference to PlayerInteraction script

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInteraction = player.GetComponent<PlayerInteraction>();
        }

        objectRenderer = GetComponentInChildren<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogError($"Renderer not found in child objects of {gameObject.name}");
            return;
        }

        materials = objectRenderer.materials;

        Debug.Log($"{gameObject.name} materials count: {materials.Length}");
        for (int i = 0; i < materials.Length; i++)
        {
            Debug.Log($"Material {i}: {materials[i]?.name ?? "null"}");
        }

        if (materials.Length < 2)
        {
            Debug.LogError($"{gameObject.name}: Expected at least 2 materials, but found {materials.Length}");
            return;
        }

        SetOutlineVisibility(false);
    }


    private void Update()
    {
        // Check if the player can interact with this object
        if (playerInteraction != null)
        {
            // Check the tag of the current object and set outline visibility based on interaction conditions
            if (CanInteractWithObject())
            {
                // Check for specific object tag and enable the outline
                if (gameObject.CompareTag("CuttingBoard") && CanInteractWithCuttingBoard())
                {
                    SetOutlineVisibility(true);
                }
                else if (gameObject.CompareTag("Stove") && CanInteractWithStove())
                {
                    SetOutlineVisibility(true);
                }
                else if (gameObject.CompareTag("Sink") && CanInteractWithWashingStation())
                {
                    SetOutlineVisibility(true);
                }
                else
                {
                    SetOutlineVisibility(false); // If no condition is met, disable the outline
                }
            }
            else
            {
                SetOutlineVisibility(false); // If the player can't interact, hide the outline
            }
        }
        else
        {
            Debug.Log("Could not find player interaction");
        }
    }

    // Check if the player can interact with the current object based on their state
    private bool CanInteractWithObject()
    {
        // Cutting Board
        bool canInteractWithCuttingBoard =
            (playerInteraction.isHolding && playerInteraction.currentItem.isCuttable && !playerInteraction.currentItem.isCut) ||
            (!playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isCut);

        // Stove
        bool canInteractWithStove =
            (playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isCookable && !playerInteraction.currentItem.isCooked) ||
            (playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isPlate && playerInteraction.currentItem.isWashed);

        // Washing Station (Sink)
        bool canInteractWithWashingStation =
            (playerInteraction.isHolding && playerInteraction.currentItem.isWashable && !playerInteraction.currentItem.isWashed) ||
            (!playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isWashed);

        // Log the results for debugging
        Debug.Log($"Can Interact With CuttingBoard: {canInteractWithCuttingBoard}");
        Debug.Log($"Can Interact With Stove: {canInteractWithStove}");
        Debug.Log($"Can Interact With WashingStation: {canInteractWithWashingStation}");

        // Return true if any of the interaction conditions are true
        return canInteractWithCuttingBoard || canInteractWithStove || canInteractWithWashingStation;
    }

    // Methods for checking interaction conditions for specific objects
    private bool CanInteractWithCuttingBoard()
    {
        return (playerInteraction.isHolding && playerInteraction.currentItem.isCuttable && !playerInteraction.currentItem.isCut) ||
               (!playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isCut);
    }

    private bool CanInteractWithStove()
    {
        return (playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isCookable && !playerInteraction.currentItem.isCooked) ||
               (playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isPlate && playerInteraction.currentItem.isWashed);
    }

    private bool CanInteractWithWashingStation()
    {
        return (playerInteraction.isHolding && playerInteraction.currentItem.isWashable && !playerInteraction.currentItem.isWashed) ||
               (!playerInteraction.isHolding && playerInteraction.currentItem != null && playerInteraction.currentItem.isWashed);
    }

    // Call this method to enable or disable the outline
    public void SetOutlineVisibility(bool visible)
    {
        if (materials.Length < 2)
        {
            Debug.LogError($"Materials array is invalid for {gameObject.name}. Expected at least 2 materials, but found {materials.Length}");
            return;
        }

        // Replace the outline material with the base material when visibility is false
        Material[] updatedMaterials = new Material[]
        {
        materials[0], // Always keep the base material
        visible ? materials[1] : materials[0] // Use outline or base material as the second slot
        };

        // Reassign the updated materials to the renderer
        objectRenderer.materials = updatedMaterials;

        Debug.Log($"{gameObject.name}: Outline visibility set to {visible}. Materials updated.");
    }


}
