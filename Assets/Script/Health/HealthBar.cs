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
    
    private UIManager _uiManager;
    
    private bool _isDead;
    private Animator _anim;
    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = originHealth;
        _anim = GetComponent<Animator>();
        _uiManager = FindFirstObjectByType<UIManager>();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, originHealth);
        if (CurrentHealth > 0)
        {
            //* Hurt animation
            _anim.SetTrigger("Hurt"); // Apply for both player and enemy
        }
        else
        {
            if (!_isDead)
            {
                _isDead = true;
                //* Play SFX
                
                
                //* Die
                if (GetComponent<Player>() != null)
                {
                    GetComponent<Player>().enabled = false;
                    GetComponent<Aim>().enabled = false;
                    
                    //* Play animation
                    _anim.SetTrigger("Die");
                    
                    _uiManager.GameOver();
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
        CurrentHealth = originHealth;
        _isDead = false;
        GetComponent<Player>().enabled = true;
        GetComponent<Aim>().enabled = true;

        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = true;
        }
        
        _anim.SetTrigger("Idle");
    }
}
