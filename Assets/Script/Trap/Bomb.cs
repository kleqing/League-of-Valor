using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Expliosion Settings")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float fuseTime;
    [SerializeField] private LayerMask damageableLayer;
    
    private Animator animator;
    
    private bool isExploded = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Explode()
    {
        if (isExploded)
        {
            return;
        }
        
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageableLayer);
        foreach (var hitCollider in hitColliders)
        {
            HealthBar healthBar = hitCollider.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetTrigger("Active");
            }
            Invoke(nameof(Explode), fuseTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
