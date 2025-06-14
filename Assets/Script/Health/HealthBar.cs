using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthBar : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] public float originHealth;

    [Header("Enemy Drop")] 
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject health;
    [SerializeField] private float dropChance;
    [SerializeField] private float healthDropChance;
    
    [Header("Death Effect")]
    [SerializeField] private GameObject deathEffect;
    
    private bool isDead;
    private Animator anim;
    public float currentHealth { get; private set; }

    private void Awake()
    {
        currentHealth = originHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, originHealth);
        if (currentHealth > 0)
        {
            //* Hurt animation
            anim.SetTrigger("Hurt"); // Apply for both player and enemy
        }
        else
        {
            if (!isDead)
            {
                isDead = true;
                //* Play SFX
                
                
                //* Die
                if (GetComponent<Player>() != null)
                {
                    GetComponent<Player>().enabled = false;
                    GetComponent<Aim>().enabled = false;
                    
                    //* Play animation
                    anim.SetTrigger("Die");
                }

                else
                {
                    if (deathEffect != null)
                    {
                        Destroy(gameObject);
                        Instantiate(deathEffect, transform.position, Quaternion.identity);
                        DropItem();
                    }
                }
            
                if (GetComponent<Collider2D>() != null)
                {
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }

    private void DropItem()
    {
        float random = Random.value;
        
        if (random <= dropChance)
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
        }
        else if (random <= healthDropChance + dropChance)
        {
            Instantiate(health, transform.position, Quaternion.identity);
        }
    }
    
    public void Revive()
    {
        currentHealth = originHealth;
        isDead = false;
        GetComponent<Player>().enabled = true;
        GetComponent<Aim>().enabled = true;

        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = true;
        }
        
        anim.SetTrigger("Idle");
    }
}
