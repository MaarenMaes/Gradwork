using UnityEngine;

public class PlateDisposer : MonoBehaviour
{
    public GameObject platePrefab; // Plate prefab to spawn
    public GameObject[] spawnPoints; // Array of spawn point GameObjects

    private Transform playerSnappingPoint; // Reference to the player's snapping point
    private bool isPlayerInTrigger = false; // Tracks if the player is in the trigger zone
    private Vector3 plateOriginalScale; // Save the original scale of the plate
    private InteractUI interactUIScript;

    [SerializeField] private bool canDisposePlate = false; // Flag to check if the player can dispose a plate
    [SerializeField] private float disposeCooldownTime = 0.2f; // Time in seconds to wait before allowing another dispose
    private float disposeCooldownTimer = 0f; // Timer to track the cooldown time

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerSnappingPoint = playerObject.transform.Find("SnappingPoint");
            interactUIScript = playerObject.GetComponent<InteractUI>();
        }

        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                GameObject plateInstance = Instantiate(platePrefab, spawnPoint.transform.position, Quaternion.Euler(0, 0, 90), spawnPoint.transform);
                plateOriginalScale = plateInstance.transform.localScale;
                plateInstance.transform.localScale = Vector3.one;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInteraction player = other.GetComponent<PlayerInteraction>();
        if (player != null)
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInTrigger = false;
        if (interactUIScript != null)
        {
            interactUIScript.ShowInteractUI = false;
        }
    }

    private void Update()
    {
        if (!canDisposePlate)
        {
            disposeCooldownTimer -= Time.deltaTime;
            if (disposeCooldownTimer <= 0f)
            {
                canDisposePlate = true;
            }
        }

        if (isPlayerInTrigger)
        {
            PickupPlate();
            DisposePlate();
        }
    }

    private void PickupPlate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerInteraction playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

            if (playerInteraction != null && playerInteraction.currentItem == null)
            {
                foreach (GameObject spawnPoint in spawnPoints)
                {
                    if (spawnPoint.transform.childCount > 0)
                    {
                        Transform plateTransform = spawnPoint.transform.GetChild(0);

                        plateTransform.SetParent(playerSnappingPoint);
                        plateTransform.localPosition = Vector3.zero;
                        plateTransform.localRotation = Quaternion.identity;
                        plateTransform.localScale = plateOriginalScale;

                        Item plateItem = plateTransform.GetComponent<Item>();
                        playerInteraction.currentItem = plateItem;

                        canDisposePlate = false;
                        disposeCooldownTimer = disposeCooldownTime;

                        if (interactUIScript != null)
                        {
                            interactUIScript.ShowInteractUI = false;
                        }

                        return;
                    }
                }
            }
        }
    }

    private void DisposePlate()
    {
        if (Input.GetKeyDown(KeyCode.E) && canDisposePlate)
        {
            PlayerInteraction playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

            if (playerInteraction != null && playerInteraction.currentItem != null && playerInteraction.currentItem.isWashed && playerInteraction.currentItem.isPlate)
            {
                foreach (GameObject spawnPoint in spawnPoints)
                {
                    if (spawnPoint.transform.childCount == 0)
                    {
                        Destroy(playerInteraction.currentItem.gameObject);
                        playerInteraction.currentItem = null;

                        GameObject newPlate = Instantiate(platePrefab, spawnPoint.transform.position, Quaternion.Euler(0, 0, 90), spawnPoint.transform);
                        newPlate.transform.localScale = Vector3.one;

                        canDisposePlate = false;
                        disposeCooldownTimer = disposeCooldownTime;

                        if (interactUIScript != null)
                        {
                            interactUIScript.ShowInteractUI = false;
                        }

                        return;
                    }
                }
            }
        }
    }
}
