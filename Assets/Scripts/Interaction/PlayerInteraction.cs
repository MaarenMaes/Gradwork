using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Item currentItem; // The item the player is holding (if any)
    public bool isHolding = false; // Check if the player is currently holding an item
    public Transform playerSnappingPoint; // Snapping point where the item should snap to the player

    [SerializeField] private GameObject dirtyPlate;

    private Animator animator; // The Animator component
    private InteractUI interactUIScript; // UI Script for interaction
    private Collider interactableCollider; // The current interactable collider

    private void Awake()
    {
        interactUIScript = this.gameObject.GetComponent<InteractUI>();
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found in child objects!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactableCollider != null)
        {
            InteractWithObject(interactableCollider);
        }

        UpdateIsHoldingState();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entered collider is an interactable object
        if (other.TryGetComponent(out CuttingBoard cuttingBoard))
        {
            if (CanInteractWithCuttingBoard(cuttingBoard))
            {
                interactableCollider = other;
                interactUIScript.ShowInteractUI = true;
            }
        }
        else if (other.TryGetComponent(out Stove stove))
        {
            if (CanInteractWithStove(stove))
            {
                interactableCollider = other;
                interactUIScript.ShowInteractUI = true;
            }
        }
        else if (other.TryGetComponent(out WashingStation washingStation))
        {
            if (CanInteractWithWashingStation(washingStation))
            {
                interactableCollider = other;
                interactUIScript.ShowInteractUI = true;
            }
        }
        else if (other.TryGetComponent(out NpcOrder npcOrder))
        {
            if (CanInteractWithNpcOrder(npcOrder))
            {
                interactableCollider = other;
                interactUIScript.ShowInteractUI = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If leaving the collider, reset the interactable reference
        if (other == interactableCollider)
        {
            interactableCollider = null;
            interactUIScript.ShowInteractUI = false;
        }
    }

    private void InteractWithObject(Collider collider)
    {
        // Interact based on object type
        if (collider.TryGetComponent(out CuttingBoard cuttingBoard))
        {
            HandleCuttingBoardInteraction(cuttingBoard);
        }
        else if (collider.TryGetComponent(out Stove stove))
        {
            HandleStoveInteraction(stove);
        }
        else if (collider.TryGetComponent(out WashingStation washingStation))
        {
            HandleWashingStationInteraction(washingStation);
        }
        else if (collider.TryGetComponent(out NpcOrder npcOrder))
        {
            HandleNpcOrderInteraction(npcOrder);
        }
    }

    // ====== Interaction Condition Checks ====== //
    private bool CanInteractWithCuttingBoard(CuttingBoard cuttingBoard)
    {
        // Show UI only if player can interact with the cutting board
        return (isHolding && currentItem.isCuttable && !currentItem.isCut) || // Holding a cuttable item
               (!isHolding && cuttingBoard.GetCutItem() != null); // Picking up a cut item
    }

    private bool CanInteractWithStove(Stove stove)
    {
        // Show UI only if player can interact with the stove
        return (isHolding && currentItem != null && currentItem.isCookable && !currentItem.isCooked) || // Holding a cookable item
               (isHolding && currentItem != null && currentItem.isPlate && currentItem.isWashed); // Holding a plate to place cooked food
    }

    private bool CanInteractWithWashingStation(WashingStation washingStation)
    {
        // Show UI only if player can interact with the washing station
        return (isHolding && currentItem.isWashable && !currentItem.isWashed) || // Holding a washable item
               (!isHolding && washingStation.GetWashedItem() != null); // Picking up a washed item
    }

    private bool CanInteractWithNpcOrder(NpcOrder npcOrder)
    {
        // Show UI only if player can interact with the NPC (correct recipe on plate)
        return isHolding &&
               ((npcOrder.isRecipe1OnPlate && currentItem.IsRecipe1ProductOnPlate) || // Recipe 1 correct
                (npcOrder.isRecipe2OnPlate && currentItem.IsRecipe2ProductOnPlate)); // Recipe 2 correct
    }

    // ====== Specific Interaction Handling ====== //

    private void HandleCuttingBoardInteraction(CuttingBoard cuttingBoard)
    {
        if (isHolding && currentItem.isCuttable && !currentItem.isCut)
        {
            if (cuttingBoard.HasFreeSnappingPoint())
            {
                cuttingBoard.CutItem(currentItem);
                currentItem = null;
                isHolding = false;
            }
        }
        else if (!isHolding)
        {
            Item cutItem = cuttingBoard.GetCutItem();
            if (cutItem != null)
            {
                PickUpItem(cutItem);
                cuttingBoard.UnSnapItem(cutItem);
            }
        }
    }

    private void HandleStoveInteraction(Stove stove)
    {
        if (isHolding && currentItem != null && currentItem.isCookable && !currentItem.isCooked)
        {
            if (stove.HasFreeSnappingPoint())
            {
                stove.CookItem(currentItem);
                currentItem = null;
                isHolding = false;
            }
        }
        else if (isHolding && currentItem != null && currentItem.isPlate && currentItem.isWashed)
        {
            GameObject recipeOnPlate = stove.SpawnRecipeOnPlate();

            if (recipeOnPlate != null)
            {
                Destroy(currentItem.gameObject);
                currentItem = null;

                GameObject instantiatedPlate = Instantiate(recipeOnPlate, playerSnappingPoint.position, Quaternion.identity);
                instantiatedPlate.transform.SetParent(playerSnappingPoint);
                isHolding = true;
                currentItem = instantiatedPlate.GetComponent<Item>();
            }
        }
    }

    private void HandleWashingStationInteraction(WashingStation washingStation)
    {
        if (isHolding && currentItem.isWashable && !currentItem.isWashed)
        {
            if (washingStation.HasFreeSnappingPoint())
            {
                washingStation.WashItem(currentItem);
                currentItem = null;
                isHolding = false;
            }
        }
        else if (!isHolding)
        {
            Item washedItem = washingStation.GetWashedItem();
            if (washedItem != null)
            {
                PickUpItem(washedItem);
                washingStation.UnSnapItem(washedItem);
            }
        }
    }

    private void HandleNpcOrderInteraction(NpcOrder npcOrder)
    {
        if (isHolding)
        {
            if (npcOrder.isRecipe1OnPlate && currentItem.IsRecipe1ProductOnPlate)
            {
                Destroy(currentItem.gameObject);
                currentItem = null;
                isHolding = false;

                npcOrder.CompleteOrder();

                GameObject instantiatedDirtyPlate = Instantiate(dirtyPlate, playerSnappingPoint.position, Quaternion.identity);
                instantiatedDirtyPlate.transform.SetParent(playerSnappingPoint);
                isHolding = true;
                currentItem = instantiatedDirtyPlate.GetComponent<Item>();
            }
            else if (npcOrder.isRecipe2OnPlate && currentItem.IsRecipe2ProductOnPlate)
            {
                Destroy(currentItem.gameObject);
                currentItem = null;
                isHolding = false;

                npcOrder.CompleteOrder();

                GameObject instantiatedDirtyPlate = Instantiate(dirtyPlate, playerSnappingPoint.position, Quaternion.identity);
                instantiatedDirtyPlate.transform.SetParent(playerSnappingPoint);
                isHolding = true;
                currentItem = instantiatedDirtyPlate.GetComponent<Item>();
            }
            else
            {
                Debug.Log("This is not the correct recipe.");
            }
        }
    }

    private void PickUpItem(Item item)
    {
        item.SnapToPoint(playerSnappingPoint);
        currentItem = item;
        isHolding = true;
    }

    private void UpdateIsHoldingState()
    {
        isHolding = currentItem != null;
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsHoldingItem", isHolding);
        }
    }
}
