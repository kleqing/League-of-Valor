using UnityEngine;

public class WarriorGun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform gun;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    
    [Header("Enemy Settings")]
    [SerializeField] private Detector detector;
    [SerializeField] private Transform player;

    private Vector2 _worldPosition;
    private Vector2 _direction;
    private float _fireTimer;
    private float _angle;

    private void Update()
    {
        if (detector.Target != null && detector.TargetVisible)
        {
            _fireTimer -= Time.deltaTime;

            FlipTowardsPlayer();

            if (_fireTimer <= 0f)
            {
                Shoot();
                _fireTimer = fireRate;
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Vector2 dir = (detector.Target.position - firePoint.position).normalized;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
            bullet.transform.up = dir;
        }
    }

    private void FlipTowardsPlayer()
    {
        if (detector.Target == null) return;

        float direction = detector.Target.position.x - transform.position.x;

        if (direction != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

}