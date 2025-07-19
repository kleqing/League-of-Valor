using JetBrains.Annotations;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Header("Patrol Settings")] 
    [SerializeField]
    [CanBeNull]
    private Transform pointA;
    [SerializeField] 
    [CanBeNull] 
    private Transform pointB;

    [Header("Enemy Settings")]
    [SerializeField] private Transform enemy;

    [Header("Movement")]
    [SerializeField] private float speed;
    
    [Header("Idle Behavior")]
    [SerializeField] private float idleDuration;
    
    private Vector3 initialScale;
    private bool isMoveLeft;
    private float idleTimer;
    
    //private Animator animator;

    private void Awake()
    {
        initialScale = enemy.localScale;
        //animator = enemy.GetComponent<Animator>();
    }

    private void Update()
    {
        if (pointA == null || pointB == null || enemy == null)
        {
            return;
        }
        if (isMoveLeft)
        {
            if (enemy.position.x >= pointA.position.x)
            {
                MoveInDirection(-1f);
            }
            else
            {
                ChangeDirection();
            }
        }
        else
        {
            if (enemy.position.x <= pointB.position.x)
            {
                MoveInDirection(1f);
            }
            else
            {
                ChangeDirection();
            }
        }
    }

    private void ChangeDirection()
    {
        //animator.SetBool("Run", false);
        idleTimer += Time.deltaTime;
        
        if (idleTimer >= idleDuration)
        {
            isMoveLeft = !isMoveLeft;
        }
    }

    private void MoveInDirection(float direction)
    {
        idleTimer = 0;
        //animator.SetBool("Run", true);
        enemy.localScale = new Vector3(Mathf.Abs(initialScale.x) * direction, initialScale.y, initialScale.z);
        
        //* Move enemy between point A and point B
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y, enemy.position.z);
    }

}