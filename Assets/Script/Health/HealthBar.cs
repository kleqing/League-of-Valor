using UnityEngine;
using Random = UnityEngine.Random;

public class HealthBar : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] public float originHealth;
    
    [Header("Zombie Drop")] 
    [SerializeField] public GameObject coin;
    [SerializeField] public GameObject gem;
    [SerializeField] private float dropChance;
    [SerializeField] private float gemDropChance;
    
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
            anim.SetTrigger("Hurt"); // Apply for both player and zombie
        }
        else
        {
            if (!isDead)
            {
                isDead = true;
                //* Play SFX
                
                //* Play animation
                anim.SetTrigger("Die");
                
                //* Die
                if (GetComponent<Player>() != null)
                {
                    GetComponent<Player>().enabled = false;
                    GetComponent<Aim>().enabled = false;
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
            Instantiate(coin, transform.position, Quaternion.identity);
        }
        else if (random <= gemDropChance + dropChance)
        {
            Instantiate(gem, transform.position, Quaternion.identity);
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
