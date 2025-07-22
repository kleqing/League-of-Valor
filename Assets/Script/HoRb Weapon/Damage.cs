using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] public float damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //* Is the layer of the other object in the layer mask?
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            //* Damage the other object
            HealthBar health = other.GetComponent<HealthBar>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
