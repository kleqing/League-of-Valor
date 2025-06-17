using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Warrior : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Detector detector;

    private float cooldownTimer = Mathf.Infinity;
    
    private HealthBar _healthBar;
    private Patrol _patrol;

    private void Awake()
    {
        _patrol = GetComponentInParent<Patrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (detector.Target != null && detector.TargetVisible)
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0f;
            }
            
            if (_patrol != null)
            {
                _patrol.enabled = false; //* Disable patrol when a target is visible
            }
        }
        else
        {
            if (_patrol != null)
            {
                _patrol.enabled = true; //* Enable patrol when no target is visible
            }
        }
    }
}