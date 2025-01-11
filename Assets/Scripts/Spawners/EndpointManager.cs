using UnityEngine;
using System.Collections.Generic;

public class EndpointManager : MonoBehaviour
{
    private static List<Transform> endpoints = new List<Transform>();
    private static List<Transform> occupiedEndpoints = new List<Transform>();

    void Awake()
    {
        // Populate endpoints
        GameObject endpointContainer = GameObject.FindGameObjectWithTag("Endpoints");
        if (endpointContainer != null)
        {
            foreach (Transform endpoint in endpointContainer.GetComponentsInChildren<Transform>())
            {
                if (endpoint != endpointContainer.transform)
                {
                    endpoints.Add(endpoint);
                }
            }
        }
        else
        {
            Debug.LogError("Endpoint container not found.");
        }
    }

    public static bool IsEndpoint(Transform transform)
    {
        return endpoints.Contains(transform);
    }

    public static bool IsEndpointOccupied(Transform endpoint)
    {
        return occupiedEndpoints.Contains(endpoint);
    }

    public static void MarkEndpointAsOccupied(Transform endpoint)
    {
        if (!occupiedEndpoints.Contains(endpoint))
        {
            occupiedEndpoints.Add(endpoint);
        }
    }

    public static void ReleaseEndpoint(Transform endpoint)
    {
        if (occupiedEndpoints.Contains(endpoint))
        {
            occupiedEndpoints.Remove(endpoint);
        }
    }

    public static Transform ReserveFreeEndpoint()
    {
        foreach (var endpoint in endpoints)
        {
            if (!IsEndpointOccupied(endpoint))
            {
                MarkEndpointAsOccupied(endpoint);
                return endpoint;
            }
        }

        return null; // No free endpoints
    }

    public static bool AreEndpointsAvailable()
    {
        return occupiedEndpoints.Count < endpoints.Count;
    }
}
