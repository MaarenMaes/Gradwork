using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private float spawnInterval = 5f;

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnNPC();
        }
    }

    private void TrySpawnNPC()
    {
        Transform freeEndpoint = EndpointManager.ReserveFreeEndpoint();

        if (freeEndpoint != null)
        {
            // Spawn the NPC
            GameObject newNpc = Instantiate(npcPrefab, transform.position, Quaternion.identity);

            // Assign the endpoint to the NPC
            NpcMovement npcMovement = newNpc.GetComponent<NpcMovement>();
            if (npcMovement != null)
            {
                npcMovement.AssignEndpoint(freeEndpoint);
            }

            //Debug.Log("Spawned new NPC and assigned an endpoint.");
        }
        else
        {
            //Debug.LogWarning("No free endpoints available. No NPC will be spawned.");
        }
    }
}
