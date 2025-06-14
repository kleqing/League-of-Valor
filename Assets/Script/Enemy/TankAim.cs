using System;
using UnityEngine;

public class TankAim : MonoBehaviour
{
    [Header("Aim Settings")] 
    [SerializeField] private GameObject bullets;
    [SerializeField] private Transform bulletSpawnPoint;
    [Range(0f, 2f)] 
    [SerializeField] private float fireRate;
    [SerializeField] private float rotationOffset = -90f;
    
    private float fireTimer = 0f;
    private Detector detector;
    private UIManager uiManager;

    private void Awake()
    {
        detector = GetComponent<Detector>();
        uiManager = FindAnyObjectByType<UIManager>();
    }

    private void Update()
    {
        if (uiManager != null && uiManager.IsPaused)
        {
            return;
        }

        if (detector.Target != null && detector.TargetVisible && detector != null)
        {
            fireTimer -= Time.deltaTime;
            AimAtTarget();
            
            if (fireTimer <= 0f && detector.TargetVisible)
            {
                Fire();
                fireTimer = fireRate;
            }
        }
    }

    private void AimAtTarget()
    {
        if (detector.Target == null)
        {
            return;
        }
        
        Vector2 direction = (detector.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
        
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    
    private void Fire()
    {
        if (bullets != null && bulletSpawnPoint != null)
        {
            Instantiate(bullets, bulletSpawnPoint.position, transform.rotation);
        }
    }
}
