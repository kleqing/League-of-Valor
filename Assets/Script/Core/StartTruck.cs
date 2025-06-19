using System.Collections.Generic;
using UnityEngine;

public class StartTruck : MonoBehaviour
{
    [SerializeField] private Movement truckMovement;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private Collider2D triggerCollider;

    private void Awake()
    {
        truckMovement.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            triggerCollider.enabled = false; // Disable the trigger to prevent multiple triggers
            truckMovement.SetWaypoint(waypoints);
            truckMovement.enabled = true;
        }
    }
}
