using System.Collections.Generic;
using UnityEngine;

public class Start : MonoBehaviour
{
    [SerializeField] private Movement truckMovement;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private Collider2D triggerCollider;

    [Header("Wave Settings")] 
    [SerializeField] private GameObject firstWave;
    [SerializeField] private GameObject startBigWave;
    
    private void Awake()
    {
        firstWave.SetActive(true);
        truckMovement.enabled = false;
        startBigWave.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            triggerCollider.enabled = false; // Disable the trigger to prevent multiple triggers
            truckMovement.SetWaypoint(waypoints);
            truckMovement.enabled = true;
            firstWave.SetActive(false);
            startBigWave.SetActive(true); //* When the player enters the trigger, enable all enemies
        }
    }
}
