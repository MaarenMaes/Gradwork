using UnityEngine;
using System.Collections.Generic;

public class NpcMovement : MonoBehaviour
{
    [SerializeField] private string waypointTag = "Waypoints";
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointThreshold = 0.2f;
    [SerializeField] private Animator animator;
    [SerializeField] private Avatar animatorAvatar;

    private List<Transform> waypoints = new List<Transform>();
    private Transform currentTarget;
    private Transform finalTarget; // The final target endpoint
    private bool isAtEndpoint = false;

    // Track whether the NPC's order is completed
    private bool isOrderFilled = false;
    private bool returningToWaypoint = false;

    void Start()
    {
        // Assign avatar to the animator
        if (animatorAvatar != null && animator != null)
        {
            animator.avatar = animatorAvatar;
        }

        // Populate waypoints
        PopulateWaypoints();
        if (waypoints.Count > 0)
        {
            SetNextWaypoint();
        }
        else
        {
            enabled = false;
        }

        // Update animator to walking state on start
        UpdateAnimatorState();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            return;
        }

        // Move towards the current target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // Rotate towards the current target smoothly (if needed)
        Vector3 direction = currentTarget.position - transform.position;

        if (direction != Vector3.zero)
        {
            // Update NPC rotation to match the target rotation directly when it reaches the target
            if (Vector3.Distance(transform.position, currentTarget.position) < waypointThreshold)
            {
                transform.rotation = currentTarget.rotation; // Match the target's rotation
            }
            else
            {
                // Rotate towards the current target during movement (optional smooth rotation)
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * moveSpeed);
            }
        }

        // Check if reached the target
        if (Vector3.Distance(transform.position, currentTarget.position) < waypointThreshold)
        {
            if (IsWaypoint(currentTarget))
            {
                HandleReachedWaypoint();
            }
            else if (IsEndpoint(currentTarget))
            {
                HandleReachedEndpoint();
            }
        }
    }

    private void SetNextWaypoint()
    {
        if (waypoints.Count > 0)
        {
            currentTarget = waypoints[0];
            UpdateAnimatorState(); // Update animator to walking
        }
    }

    private void MoveToEndpoint()
    {
        if (finalTarget != null)
        {
            currentTarget = finalTarget; // Move to the assigned final endpoint
        }
        else
        {
            Transform assignedEndpoint = EndpointManager.ReserveFreeEndpoint();

            if (assignedEndpoint != null)
            {
                currentTarget = assignedEndpoint;
            }
        }

        UpdateAnimatorState(); // Update animator to walking
    }

    private void HandleReachedWaypoint()
    {
        // If returning to a waypoint after order completion, destroy NPC
        if (returningToWaypoint && isOrderFilled)
        {
            Destroy(this.gameObject);
            return;
        }

        // Move to the next waypoint if available
        waypoints.Remove(currentTarget);
        if (waypoints.Count > 0)
        {
            SetNextWaypoint();
        }
        else
        {
            // If no waypoints are left, move to the final endpoint
            MoveToEndpoint();
        }
    }

    private void HandleReachedEndpoint()
    {
        isAtEndpoint = true;
        EndpointManager.MarkEndpointAsOccupied(currentTarget);

        // If the order is not filled, NPC waits at the endpoint
        if (!isOrderFilled)
        {
            UpdateAnimatorState(); // Update animator to idle/sitting
        }
        else
        {
            // Start returning to waypoints
            returningToWaypoint = true;
            PopulateWaypoints(); // Reload waypoints
            SetNextWaypoint();   // Start heading to the first waypoint
        }
    }

    public void AssignEndpoint(Transform endpoint)
    {
        finalTarget = endpoint;
    }

    private bool IsWaypoint(Transform target)
    {
        return waypoints.Contains(target);
    }

    private bool IsEndpoint(Transform target)
    {
        return EndpointManager.IsEndpoint(target);
    }

    private void UpdateAnimatorState()
    {
        if (isAtEndpoint)
        {
            animator.SetBool("walking", false);
            animator.SetBool("sitting", true);
        }
        else
        {
            animator.SetBool("walking", true);
            animator.SetBool("sitting", false);
        }
    }

    private void OnDestroy()
    {
        if (currentTarget != null && EndpointManager.IsEndpoint(currentTarget))
        {
            EndpointManager.ReleaseEndpoint(currentTarget);  // Mark the endpoint as unoccupied
        }
    }

    // Method to refill waypoints
    private void PopulateWaypoints()
    {
        GameObject waypointContainer = GameObject.FindGameObjectWithTag(waypointTag);

        if (waypointContainer != null)
        {
            waypoints.Clear();
            Transform[] waypointTransforms = waypointContainer.GetComponentsInChildren<Transform>();

            foreach (var waypoint in waypointTransforms)
            {
                if (waypoint != waypointContainer.transform)
                {
                    waypoints.Add(waypoint);
                }
            }
        }
    }

    // Set the order as filled or not
    public void SetOrderFilled(bool filled)
    {
        isOrderFilled = filled;

        // If the order is filled and NPC is at the endpoint, start returning to waypoints
        if (isOrderFilled && isAtEndpoint)
        {
            returningToWaypoint = true;
            isAtEndpoint = false; // Reset endpoint flag
            PopulateWaypoints(); // Reload waypoints
            SetNextWaypoint();   // Start heading to the first waypoint
        }
    }
}
