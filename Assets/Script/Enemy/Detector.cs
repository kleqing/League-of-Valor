using System;
using System.Collections;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [Header("Detection Settings")] 
    [Range(1f, 15f)]
    [SerializeField] private float detectionRadius;
    [SerializeField] private float detectionDelay;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask visibilityLayer;
    [SerializeField] private Transform target = null;

    [field: SerializeField]
    public bool TargetVisible { get; private set; }

    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            TargetVisible = false;
        }
    }

    private void Start()
    {
        StartCoroutine(DetectionCoroutine());
    }

    private void Update()
    {
        if (Target != null)
        {
            TargetVisible = CheckTargetVisibility();
        }
    }

    private bool CheckTargetVisibility()
    {
        var result = Physics2D.Raycast(transform.position, Target.position - transform.position, detectionRadius, targetLayer);
        if (result.collider != null)
        {
            return (targetLayer & (1 << result.collider.gameObject.layer)) != 0;
        }

        return false;
    }

    private void DetectTarget()
    {
        if (Target == null)
        {
            IsPlayerInRange();
        }
        else if (Target != null)
        {
            DetectTargetOutOfRange();
        }
    }

    private void DetectTargetOutOfRange()
    {
        if (Target == null || Target.gameObject.activeSelf == false ||
            Vector2.Distance(transform.position, Target.position) > detectionRadius)
        {
            Target = null;
        }
    }

    private void IsPlayerInRange()
    {
        Collider2D collision = Physics2D.OverlapCircle(transform.position, detectionRadius, targetLayer);
        if (collision != null)
        {
            Target = collision.transform;
        }
    }

    private IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        DetectTarget();
        StartCoroutine(DetectionCoroutine());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
