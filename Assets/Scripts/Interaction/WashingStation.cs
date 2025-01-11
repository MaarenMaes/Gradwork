using UnityEngine;

public class WashingStation : MonoBehaviour
{
    public Transform[] washingStationSnappingPoints; // Snapping points array for the washing station
    private Item[] washingStationSnappedItems; // Store references to the items occupying the points

    public float washingTime = 4f; // Total time to wash an item
    public float currentTimer = 0f; // Timer for the current washing process

    [SerializeField] private GameObject cleanDishPrefab; // Prefab for the cleaned dish
    [SerializeField] private GameObject runningDrain; // The running drain GameObject

    private void Start()
    {
        washingStationSnappedItems = new Item[washingStationSnappingPoints.Length];

        // Ensure runningDrain is inactive at the start
        if (runningDrain != null)
        {
            runningDrain.SetActive(false);
        }
    }

    // Check if there's any free snapping point
    public bool HasFreeSnappingPoint()
    {
        for (int i = 0; i < washingStationSnappingPoints.Length; i++)
        {
            if (washingStationSnappedItems[i] == null)
            {
                return true; // At least one free snapping point
            }
        }
        return false; // No free snapping points
    }

    // Method to wash the item
    public void WashItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("No item provided to the washing station.");
            return;
        }

        if (item.isWashable && !item.isWashed)
        {
            for (int i = 0; i < washingStationSnappingPoints.Length; i++)
            {
                if (washingStationSnappedItems[i] == null) // Check if the point is available
                {
                    item.SnapToPoint(washingStationSnappingPoints[i]);
                    washingStationSnappedItems[i] = item; // Mark the point as occupied
                    currentTimer = washingTime; // Start the washing timer

                    // Activate running drain to indicate washing
                    if (runningDrain != null)
                    {
                        runningDrain.SetActive(true);
                    }

                    Debug.Log($"{item.itemName} is being washed and is now snapped to the washing station.");
                    return;
                }
            }

            Debug.LogError("No available snapping points on the washing station.");
        }
        else if (item.isWashed)
        {
            Debug.Log($"{item.itemName} is already washed.");
        }
        else
        {
            Debug.Log($"{item.itemName} cannot be washed.");
        }
    }

    // Update method to handle the washing process over time
    private void Update()
    {
        UpdateWashingProcess();
    }

    private void UpdateWashingProcess()
    {
        for (int i = 0; i < washingStationSnappedItems.Length; i++)
        {
            if (washingStationSnappedItems[i] != null && currentTimer > 0)
            {
                currentTimer -= Time.deltaTime;

                // When the washing time is up, change the item state
                if (currentTimer <= 0)
                {
                    // Replace the washed item with a clean dish prefab
                    ReplaceWithCleanDish(i);
                }
            }
        }
    }

    private void ReplaceWithCleanDish(int index)
    {
        if (washingStationSnappedItems[index] != null)
        {
            Item originalItem = washingStationSnappedItems[index];

            // Mark the original item as washed
            originalItem.WashItem();
            Debug.Log($"{originalItem.itemName} has been washed!");

            // Instantiate the clean dish prefab at the same position and rotation
            GameObject cleanDishInstance = Instantiate(
                cleanDishPrefab,
                originalItem.transform.position,
                originalItem.transform.rotation
            );

            // Set the clean dish instance as the new snapped item
            washingStationSnappedItems[index] = cleanDishInstance.GetComponent<Item>();

            // Destroy the original item
            Destroy(originalItem.gameObject);

            Debug.Log("Replaced the washed item with a clean dish prefab.");
        }

        // Deactivate the running drain since washing is complete
        if (runningDrain != null)
        {
            runningDrain.SetActive(false);
        }

        // Reset the timer
        currentTimer = 0f;
    }

    // Method to unsnap the item when removed
    public void UnSnapItem(Item item)
    {
        for (int i = 0; i < washingStationSnappingPoints.Length; i++)
        {
            if (washingStationSnappedItems[i] == item)
            {
                washingStationSnappedItems[i] = null; // Free the snapping point
                currentTimer = 0f; // Reset the timer
                Debug.Log($"{item.itemName} has been removed from the washing station.");
                break;
            }
        }
    }

    // Method to retrieve the first available washed item
    public Item GetWashedItem()
    {
        for (int i = 0; i < washingStationSnappedItems.Length; i++)
        {
            if (washingStationSnappedItems[i] != null && washingStationSnappedItems[i].isWashed)
            {
                return washingStationSnappedItems[i]; // Return the first washed item found
            }
        }
        return null; // No washed items available
    }
}
