using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField] private HealthBar playerHealth;
    [SerializeField] private Slider currentHealth;

    private void Start()
    {
        currentHealth.maxValue = playerHealth.currentHealth;
        currentHealth.value = playerHealth.currentHealth;
    }

    private void Update()
    {
        currentHealth.value = playerHealth.currentHealth;
    }
}
