using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string _itemName;

    [SerializeField] private bool _isCuttable;
    [SerializeField] private bool _isWashable;
    [SerializeField] private bool _isCookable;

    [SerializeField] private bool _isCut;
    [SerializeField] private bool _isWashed;
    [SerializeField] private bool _isCooked;
    [SerializeField] private bool _isPlate;

    [SerializeField] private bool _canBecomeCookableAfterCut = true;

    [SerializeField] private bool _isRecipe1Product = false;
    [SerializeField] private bool _isRecipe2Product = false;

    [SerializeField] private bool _isRecipe1ProductOnPlate = false;
    [SerializeField] private bool _isRecipe2ProductOnPlate = false;
    // References to the different states of the item (raw, cut, cooked, washed)
    [SerializeField] private GameObject rawObject;
    [SerializeField] private GameObject cutObject;
    [SerializeField] private GameObject cookedObject;
    [SerializeField] private GameObject washedObject;

    // Public properties for other scripts
    public string itemName { get; private set; }
    public bool isCuttable { get; private set; }
    public bool isCut { get; set; }
    public bool isPlate { get; private set; }
    public bool isWashable { get; private set; }
    public bool isWashed { get; private set; }
    public bool isCookable { get; private set; }
    public bool isCooked { get; private set; }
    public bool canBecomeCookableAfterCut { get; private set; }

    public bool IsRecipe1Product { get; private set; }
    public bool IsRecipe2Product { get; private set; }

    public bool IsRecipe1ProductOnPlate { get; private set; }
    public bool IsRecipe2ProductOnPlate { get; private set; }


    // Reference to the snapping points (will be set by interaction points)
    private Transform currentSnappingPoint; // The current snapping point the item is snapped to

    private void Start()
    {
        itemName = _itemName;
        isCuttable = _isCuttable;
        isWashable = _isWashable;
        isCookable = _isCookable;
        isCut = _isCut;
        isWashed = _isWashed;
        isCooked = _isCooked;
        canBecomeCookableAfterCut = _canBecomeCookableAfterCut;
        isPlate = _isPlate;

        IsRecipe1Product = _isRecipe1Product;
        IsRecipe2Product = _isRecipe2Product;

        IsRecipe1ProductOnPlate = _isRecipe1ProductOnPlate;
        IsRecipe2ProductOnPlate = _isRecipe2ProductOnPlate;

        // Update the item state on start
        UpdateItemState();
    }

    // Method to snap the item to a specific snapping point
    public void SnapToPoint(Transform snappingPoint)
    {
        currentSnappingPoint = snappingPoint;
        transform.position = snappingPoint.position;
        transform.rotation = snappingPoint.rotation;
        transform.SetParent(snappingPoint); // Parent the item to the snapping point
    }

    // Method to return the item to the player
    public void ReturnToPlayer(Transform playerSnappingPoint)
    {
        transform.SetParent(playerSnappingPoint); // Parent the item to the player's snapping point
        transform.position = playerSnappingPoint.position;
        transform.rotation = playerSnappingPoint.rotation;
    }

    // Method to cut the item
    public void CutItem()
    {
        if (isCuttable && !isCut)
        {
            isCut = true;
            //Debug.Log($"{itemName} has been cut.");

            // If the item can become cookable after being cut, update its cookable state
            if (canBecomeCookableAfterCut)
            {
                isCookable = true;
                //Debug.Log($"{itemName} is now cookable after being cut.");
            }

            UpdateItemState();  // Update the visual after cutting the item
        }
        else if (isCut)
        {
            //Debug.Log($"{itemName} is already cut.");
        }
        else
        {
            //Debug.Log($"{itemName} cannot be cut.");
        }
    }

    // Method to wash the item
    public void WashItem()
    {
        if (isWashable && !isWashed)
        {
            isWashed = true;
            //Debug.Log($"{itemName} has been washed.");
            UpdateItemState();  // Update the visual after washing the item
        }
        else if (isWashed)
        {
            //Debug.Log($"{itemName} is already washed.");
        }
        else
        {
            //Debug.Log($"{itemName} cannot be washed.");
        }
    }

    // Method to cook the item
    public void CookItem()
    {
        if (isCookable && !isCooked)
        {
            isCooked = true;
            //Debug.Log($"{itemName} has been cooked.");
            UpdateItemState();  // Update the visual after cooking the item
        }
        else if (isCooked)
        {
            //Debug.Log($"{itemName} is already cooked.");
        }
        else
        {
            //Debug.Log($"{itemName} cannot be cooked.");
        }
    }

    // Update the item state by toggling the active states of the raw, cut, cooked, and washed objects
    public void UpdateItemState()
    {
        // Set all item states inactive
        if (rawObject != null) rawObject.SetActive(false);
        if (cutObject != null) cutObject.SetActive(false);
        if (cookedObject != null) cookedObject.SetActive(false);
        if (washedObject != null) washedObject.SetActive(false);

        // Activate the correct state based on the current condition of the item
        if (isCooked && cookedObject != null)
        {
            cookedObject.SetActive(true);
        }
        else if (isCut && cutObject != null)
        {
            cutObject.SetActive(true);
        }
        else if (isWashed && washedObject != null)
        {
            washedObject.SetActive(true);
        }
        else if (rawObject != null)
        {
            rawObject.SetActive(true);
        }
    }

    private void Update()
    {
        // If the item is snapped to a point, it should follow the snapping point's position and rotation
        if (currentSnappingPoint != null)
        {
            transform.position = currentSnappingPoint.position;
            transform.rotation = currentSnappingPoint.rotation;
        }
    }
}
