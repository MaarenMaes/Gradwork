using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class Stove : MonoBehaviour
{
    public Transform[] stoveSnappingPoints; // Snapping points array for the stove
    private GameObject[] stoveSnappedItems; // Store references to the items occupying the points
    public float cookingTime = 5f; // Time to cook an item in seconds
    public float itemCookTimer; // Timer for the current item cooking

    [SerializeField] private MeshFilter[] recipe1Items; // MeshFilters required for Recipe 1
    [SerializeField] private MeshFilter[] recipe2Items; // MeshFilters required for Recipe 2

    [SerializeField] private GameObject recipe1Product; // Finished product for Recipe 1
    [SerializeField] private GameObject recipe2Product; // Finished product for Recipe 2

    [SerializeField] private GameObject recipe1ProductOnPlate; // Finished product for Recipe 1
    [SerializeField] private GameObject recipe2ProductOnPlate; // Finished product for Recipe 2

    [SerializeField] private GameObject fireParticleEffect;
    [SerializeField] private float StoveTimeOutTime = 10f;

    private void Start()
    {
        stoveSnappedItems = new GameObject[stoveSnappingPoints.Length];
    }

    // Check if there's any free snapping point
    public bool HasFreeSnappingPoint()
    {
        for (int i = 0; i < stoveSnappingPoints.Length; i++)
        {
            if (stoveSnappedItems[i] == null)
            {
                return true; // At least one free snapping point
            }
        }
        return false; // No free snapping points
    }

    // Method to cook the item
    public void CookItem(Item item)
    {
        if (item == null)
        {
            //Debug.LogError("No item provided to the stove.");
            return;
        }

        if (item.isCookable && !item.isCooked)
        {
            for (int i = 0; i < stoveSnappingPoints.Length; i++)
            {
                if (stoveSnappedItems[i] == null) // Check if the point is available
                {
                    item.SnapToPoint(stoveSnappingPoints[i]);
                    stoveSnappedItems[i] = item.gameObject; // Store the game object
                    itemCookTimer = cookingTime; // Start the cooking timer
                    //Debug.Log($"{item.itemName} is cooking and is now snapped to the stove.");
                    return;
                }
            }
            //Debug.LogError("No available snapping points on the stove.");
        }
        else if (item.isCooked)
        {
            //Debug.Log($"{item.itemName} is already cooked.");
        }
        else
        {
            //Debug.Log($"{item.itemName} cannot be cooked.");
        }
    }

    // Update method to handle the cooking process over time
    public void Update()
    {
        for (int i = 0; i < stoveSnappedItems.Length; i++)
        {
            if (stoveSnappedItems[i] != null)
            {
                // Check if the item is cooking and there is a timer set
                if (itemCookTimer > 0)
                {
                    itemCookTimer -= Time.deltaTime;

                    // When the cooking time is up, change the item state
                    if (itemCookTimer <= 0)
                    {
                        // Mark the item as cooked and update its state
                        Item itemComponent = stoveSnappedItems[i].GetComponent<Item>();
                        if (itemComponent != null)
                        {
                            itemComponent.CookItem();
                            //Debug.Log($"{itemComponent.itemName} has finished cooking!");
                            CheckRecipes(); // Check if any recipes are completed
                            NoRecipeFound();
                        }
                    }
                }
            }
        }
    }

    // Method to check if any recipes are completed
    private void CheckRecipes()
    {
        bool recipe1Matched = CheckRecipeMatch(recipe1Items);
        bool recipe2Matched = CheckRecipeMatch(recipe2Items);

        if (recipe1Matched)
        {
            //Debug.Log("Recipe 1 is completed!");
            ClearStoveItems();
            SpawnFinishedProduct(recipe1Product, FindEmptySnappingPoint());
        }
        else if (recipe2Matched)
        {
            //Debug.Log("Recipe 2 is completed!");
            ClearStoveItems();
            SpawnFinishedProduct(recipe2Product, FindEmptySnappingPoint());
        }
    }

    // Method to check if the currently snapped items match a given recipe
    private bool CheckRecipeMatch(MeshFilter[] recipe)
    {
        if (recipe.Length != stoveSnappingPoints.Length)
        {
            //Debug.LogError("Recipe item count does not match stove snapping points!");
            return false;
        }

        // Convert stove snapped items to a list of active mesh names
        var activeMeshNames = new HashSet<string>();
        foreach (var snappedItem in stoveSnappedItems)
        {
            if (snappedItem != null)
            {
                MeshFilter meshFilter = snappedItem.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.gameObject.activeSelf)
                {
                    activeMeshNames.Add(meshFilter.sharedMesh.name);
                }
            }
        }

        // Check if all required meshes are present
        foreach (var item in recipe)
        {
            if (item != null && item.sharedMesh != null)
            {
                if (!activeMeshNames.Contains(item.sharedMesh.name))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Method to spawn the finished product
    private void SpawnFinishedProduct(GameObject productPrefab, Transform spawnPoint)
    {
        if (productPrefab != null && spawnPoint != null)
        {
            // Instantiate the product
            GameObject spawnedProduct = Instantiate(productPrefab, spawnPoint.position, Quaternion.identity);

            // Now snap the product to the stove's snapping point
            Item itemComponent = spawnedProduct.GetComponent<Item>();
            if (itemComponent != null)
            {
                // Find the first empty snapping point
                for (int i = 0; i < stoveSnappingPoints.Length; i++)
                {
                    if (stoveSnappedItems[i] == null)
                    {
                        // Snap the item to this point
                        itemComponent.SnapToPoint(stoveSnappingPoints[i]);
                        stoveSnappedItems[i] = spawnedProduct; // Register the item at the snapping point
                        //Debug.Log($"{productPrefab.name} has been snapped to the stove.");
                        break;
                    }
                }
            }
        }
        else
        {
            //Debug.LogError("Product prefab not assigned or no empty snapping point available for spawning!");
        }
    }

    // Method to find an empty snapping point
    private Transform FindEmptySnappingPoint()
    {
        foreach (var snappingPoint in stoveSnappingPoints)
        {
            bool isOccupied = false;
            for (int i = 0; i < stoveSnappedItems.Length; i++)
            {
                if (stoveSnappedItems[i] != null && stoveSnappedItems[i].transform == snappingPoint)
                {
                    isOccupied = true;
                    break;
                }
            }
            if (!isOccupied)
            {
                return snappingPoint;
            }
        }
        return null; // No empty snapping points found
    }

    // Method to unsnap all items from the stove
    private void ClearStoveItems()
    {
        for (int i = 0; i < stoveSnappingPoints.Length; i++)
        {
            if (stoveSnappedItems[i] != null)
            {
                Destroy(stoveSnappedItems[i]); // Destroy the snapped item
                stoveSnappedItems[i] = null; // Free the snapping points
            }
        }
        itemCookTimer = 0f; // Reset the timer
    }

    // Method to unsnap the item when removed
    public void UnSnapItem(Item item)
    {
        for (int i = 0; i < stoveSnappingPoints.Length; i++)
        {
            if (stoveSnappedItems[i] == item.gameObject)
            {
                Destroy(stoveSnappedItems[i]); // Destroy the snapped item
                stoveSnappedItems[i] = null; // Free the snapping point
                itemCookTimer = 0f; // Reset the timer if needed
                break;
            }
        }
    }

    // Method to spawn the recipe on the plate (when interacting with the stove)
    public GameObject SpawnRecipeOnPlate()
    {
        // Iterate through the snapping points to check if any recipe product is placed
        for (int i = 0; i < stoveSnappingPoints.Length; i++)
        {
            if (stoveSnappedItems[i] != null)
            {
                // Get the Item component from the snapped object
                Item itemComponent = stoveSnappedItems[i].GetComponent<Item>();

                if (itemComponent != null)
                {
                    // Check if this is Recipe 1 product
                    if (itemComponent.IsRecipe1Product)
                    {

                        Destroy(itemComponent.gameObject);
                        //Debug.Log("Recipe 1 is on the stove, returning recipe 1 on plate.");
                        return recipe1ProductOnPlate; // Return the recipe1ProductOnPlate
                    }
                    // Check if this is Recipe 2 product
                    else if (itemComponent.IsRecipe2Product)
                    {

                        Destroy(itemComponent.gameObject);
                        //Debug.Log("Recipe 2 is on the stove, returning recipe 2 on plate.");
                        return recipe2ProductOnPlate; // Return the recipe2ProductOnPlate
                    }
                }
            }
        }

        // If no matching product found, return null
        //Debug.Log("No recipe product found on the stove.");
        return null;
    }
    // Method to check if there are exactly 3 items on the stove and no recipe matches
    // Method to check if there are exactly 3 items on the stove and no recipe matches
    public void NoRecipeFound()
    {
        // Count the number of items on the stove
        int itemsOnStove = 0;
        foreach (var item in stoveSnappedItems)
        {
            if (item != null)
            {
                itemsOnStove++;
            }
        }

        // Only proceed if there are exactly 3 items on the stove
        if (itemsOnStove == 3)
        {
            // Check if it's a valid recipe
            bool recipe1Matched = CheckRecipeMatch(recipe1Items);
            bool recipe2Matched = CheckRecipeMatch(recipe2Items);

            if (!recipe1Matched && !recipe2Matched)
            {
                // If no recipe matches, log the message and destroy all items on the stove
                Debug.Log("No correct recipe");

                // Trigger fire particle effect
                if (fireParticleEffect != null)
                {
                    Transform emptySnappingPoint = FindEmptySnappingPoint();

                    if (emptySnappingPoint != null)
                    {
                        // Instantiate the fire particle effect
                        GameObject fireEffect = Instantiate(fireParticleEffect, emptySnappingPoint.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

                        // Parent the fire particle effect to the snapping point
                        fireEffect.transform.SetParent(emptySnappingPoint);

                        // Optionally: Destroy the fire effect after a certain delay
                        StartCoroutine(DestroyFireEffectAfterDelay(fireEffect, StoveTimeOutTime)); // Delay for 3 seconds (or your desired time)
                    }
                }

                // Optionally: Destroy the items on the stove after a delay (if needed)
                StartCoroutine(ClearItemsAfterDelay(StoveTimeOutTime));
            }
        }
    }

    // Coroutine to destroy the fire effect after a set delay
    private IEnumerator DestroyFireEffectAfterDelay(GameObject fireEffect, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (fireEffect != null)
        {
            // Destroy the fire effect
            Destroy(fireEffect);
            Debug.Log("Fire effect destroyed.");
        }
    }

    // Coroutine to clear the stove after 3 seconds (while fire particles are active)
    private IEnumerator ClearItemsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Clear the stove items
        ClearStoveItems();
    }

}
