using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float damage;
    
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        DestroyBullet();
        SetStraightVelocity();
    }

    private void SetStraightVelocity()
    {
        float playerDirection = Mathf.Sign(transform.lossyScale.x);
        rb.linearVelocity = transform.right * bulletSpeed * playerDirection;
    }

    private void DestroyBullet()
    {
        Destroy(gameObject, bulletLifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //* Is the layer of the other object in the layer mask?
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            //* Play SFX
            
            //* Damage the other object
            HealthBar health = other.GetComponent<HealthBar>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            
            //* Destroy the bullet
            Destroy(gameObject);
        }
    }
}
