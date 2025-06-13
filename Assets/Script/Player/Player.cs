using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float speed;
    
    [Header("Player Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashRecoveryDuration;
    
    [Header("Dash Effect")]
    [SerializeField] private GameObject dashEffect; // Prefab ghost
    [SerializeField] private float dashEffectCooldown = 0.05f; // Tần suất sinh ghost
    
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movement;
    private PlayerInput playerInput;
    
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashRecovery;
    private float dashRecoveryTimer;
    private Vector2 dashDirection;
    private Coroutine dashEffectCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
        playerInput.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled += OnMove;
        playerInput.Player.Dash.performed -= OnDash;
        playerInput.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && !isDashRecovery)
        {
            movement = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            movement = Vector2.zero;
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && dashCooldownTimer <= 0f && movement != Vector2.zero)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashDirection = movement.normalized;
            dashCooldownTimer = dashCooldown;
            
            playerInput.Player.Move.Disable(); //* Disable movement input during dash
            StartDashEffect();
        }
    }

    private void Update()
    {
        anim.SetBool("isRun", movement != Vector2.zero && !isDashing);
        anim.SetBool("isDash", isDashing);

        // Flip nhân vật
        if (isDashing)
        {
            if (dashDirection.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(dashDirection.x), 1, 1);
            }
        }
        else if (movement.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                StopDashEffect();
                
                isDashRecovery = true;
                dashRecoveryTimer = dashRecoveryDuration;
            }
        }

        if (isDashRecovery)
        {
            dashRecoveryTimer -= Time.deltaTime;
            if (dashRecoveryTimer <= 0f)
            {
                isDashRecovery = false;
                playerInput.Player.Move.Enable(); //* Re-enable movement input after dash recovery
            }
        }
        
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else if (isDashRecovery)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = movement.normalized * speed;
        }
    }

    private void StartDashEffect()
    {
        if (dashEffectCoroutine != null)
        {
            StopCoroutine(dashEffectCoroutine);
        }
        dashEffectCoroutine = StartCoroutine(DashEffectCoroutine());
    }

    private void StopDashEffect()
    {
        if (dashEffectCoroutine != null)
        {
            StopCoroutine(dashEffectCoroutine);
            dashEffectCoroutine = null;
        }
    }

    private IEnumerator DashEffectCoroutine()
    {
        while (isDashing)
        {
            GameObject effect = Instantiate(dashEffect, transform.position, transform.rotation);
            SpriteRenderer effectSpriteRenderer = effect.GetComponent<SpriteRenderer>();

            if (effectSpriteRenderer != null)
            {
                effect.transform.localScale = transform.localScale;
            }

            Destroy(effect, 0.5f);
            yield return new WaitForSeconds(dashEffectCooldown);
        }
    }
}