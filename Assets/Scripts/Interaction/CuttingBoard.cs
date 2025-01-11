using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    public Transform[] cuttingBoardSnappingPoints; // Snapping points array for the cutting board
    private Item[] cuttingBoardSnappedItems; // Store references to the items occupying the points

    public float cuttingTime = 3f; // Time it takes to cut an item
    public float itemCutTimer = 0f; // Timer counting down when an item is being cut

    private Item currentCuttingItem; // The item currently being cut

    private InteractUI interactUIScript;
    private void Awake()
    {
        interactUIScript = this.gameObject.GetComponent<InteractUI>();
    }


    private void Start()
    {
        cuttingBoardSnappedItems = new Item[cuttingBoardSnappingPoints.Length];
    }

    // Check if there's any free snapping point
    public bool HasFreeSnappingPoint()
    {
        foreach (var snappedItem in cuttingBoardSnappedItems)
        {
            if (snappedItem == null)
            {
                return true; // At least one free snapping point
            }
        }
        return false; // No free snapping points
    }

    // Method to start cutting an item
    public void CutItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("No item provided to the cutting board.");
            return;
        }

        if (item.isCuttable && !item.isCut)
        {
            for (int i = 0; i < cuttingBoardSnappingPoints.Length; i++)
            {
                if (cuttingBoardSnappedItems[i] == null) // Check if the point is available
                {
                    item.SnapToPoint(cuttingBoardSnappingPoints[i]);
                    cuttingBoardSnappedItems[i] = item; // Mark the point as occupied
                    currentCuttingItem = item; // Set the item being cut
                    itemCutTimer = cuttingTime; // Start the cutting timer
                    Debug.Log($"{item.itemName} is being cut and is now snapped to the cutting board.");
                    return;
                }
            }

            Debug.LogError("No available snapping points on the cutting board.");
        }
        else if (item.isCut)
        {
            Debug.Log($"{item.itemName} is already cut.");
        }
        else
        {
            Debug.Log($"{item.itemName} cannot be cut.");
        }
    }

    private void Update()
    {
        HandleCuttingProcess();
    }

    // Method to handle the cutting process
    private void HandleCuttingProcess()
    {
        if (currentCuttingItem != null && itemCutTimer > 0)
        {
            itemCutTimer -= Time.deltaTime;

            // When the cutting time is up
            if (itemCutTimer <= 0)
            {
                // Mark the item as cut and update its state
                currentCuttingItem.CutItem();
                Debug.Log($"{currentCuttingItem.itemName} has been cut!");

                // Reset cutting process
                currentCuttingItem = null;
                itemCutTimer = 0f;
            }
        }
    }

    // Method to unsnap the item when removed
    public void UnSnapItem(Item item)
    {
        for (int i = 0; i < cuttingBoardSnappingPoints.Length; i++)
        {
            if (cuttingBoardSnappedItems[i] == item)
            {
                cuttingBoardSnappedItems[i] = null; // Free the snapping point
                if (currentCuttingItem == item)
                {
                    currentCuttingItem = null; // Stop cutting this item
                    itemCutTimer = 0f;
                }
                Debug.Log($"{item.itemName} has been removed from the cutting board.");
                break;
            }
        }
    }

    // Method to retrieve the first available cut item
    public Item GetCutItem()
    {
        foreach (var snappedItem in cuttingBoardSnappedItems)
        {
            if (snappedItem != null && snappedItem.isCut)
            {
                return snappedItem; // Return the first cut item found
            }
        }
        return null; // No cut items available
    }
}
