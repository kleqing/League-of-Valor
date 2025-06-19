using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField] private HealthBar playerHealth;
    [SerializeField] private Slider currentHealth;

    private void Start()
    {
        currentHealth.maxValue = playerHealth.CurrentHealth;
        currentHealth.value = playerHealth.CurrentHealth;
    }

    private void Update()
    {
        currentHealth.value = playerHealth.CurrentHealth;
    }
}
