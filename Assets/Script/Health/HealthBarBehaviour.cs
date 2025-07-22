using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    private HealthBar playerHealth;
    [SerializeField] private Slider currentHealth;

    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthBar>();

        if (playerHealth == null)
        {
            return;
        }

        currentHealth.maxValue = playerHealth.CurrentHealth;
        currentHealth.value = playerHealth.CurrentHealth;
    }

    private void Update()
    {
        if (playerHealth != null)
        {
            currentHealth.value = playerHealth.CurrentHealth;
        }
    }
}
