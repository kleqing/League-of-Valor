using System;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float stopDistance;
    
    private int currentWaypointIndex = 0;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError("No waypoints assigned to the truck movement script.");
        }
    }

    private void Update()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
            
            float rotateZAngle = targetWaypoint.eulerAngles.z;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, rotateZAngle);
            
            if (Vector3.Distance(transform.position, targetWaypoint.position) < stopDistance)
            {
                currentWaypointIndex++;
            }
        }

        else
        {
            enabled = false;
        }
    }

    public void SetWaypoint(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
        enabled = true;
    }
}
