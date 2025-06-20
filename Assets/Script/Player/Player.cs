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
    [SerializeField] private GameObject dashEffect;
    [SerializeField] private float dashEffectCooldown = 0.05f;
    
    private Animator _anim;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private PlayerInput _playerInput;
    private Collider2D _collider;
    
    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private bool _isDashRecovery;
    private float _dashRecoveryTimer;
    private Vector2 _dashDirection;
    private Coroutine _dashEffectCoroutine;
    
    private static readonly string IsRun = "isRun";

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerInput = new PlayerInput();
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.canceled += OnMove;
        _playerInput.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.performed -= OnMove;
        _playerInput.Player.Move.canceled += OnMove;
        _playerInput.Player.Dash.performed -= OnDash;
        _playerInput.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && !_isDashing && !_isDashRecovery)
        {
            _movement = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _movement = Vector2.zero;
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !_isDashing && _dashCooldownTimer <= 0f && _movement != Vector2.zero)
        {
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashDirection = _movement.normalized;
            _dashCooldownTimer = dashCooldown;
            
            _playerInput.Player.Move.Disable(); //* Disable movement input during dash
            _collider.enabled = false;
            StartDashEffect();
        }
    }

    private void Update()
    {
        _anim.SetBool(IsRun, _movement != Vector2.zero && !_isDashing);
        
        if (_isDashing)
        {
            if (_dashDirection.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(_dashDirection.x), 1, 1);
            }
        }
        else if (_movement.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(_movement.x), 1, 1);
        }

        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                StopDashEffect();
                
                _isDashRecovery = true;
                _dashRecoveryTimer = dashRecoveryDuration;
            }
        }

        if (_isDashRecovery)
        {
            _dashRecoveryTimer -= Time.deltaTime;
            if (_dashRecoveryTimer <= 0f)
            {
                _isDashRecovery = false;
                _playerInput.Player.Move.Enable(); //* Re-enable movement input after dash recovery
                _collider.enabled = true; //* Re-enable collider after dash recovery
            }
        }
        
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            _rb.linearVelocity = _dashDirection * dashSpeed;
        }
        else if (_isDashRecovery)
        {
            _rb.linearVelocity = Vector2.zero;
        }
        else
        {
            _rb.linearVelocity = _movement.normalized * speed;
        }
    }

    private void StartDashEffect()
    {
        if (_dashEffectCoroutine != null)
        {
            StopCoroutine(_dashEffectCoroutine);
        }
        _dashEffectCoroutine = StartCoroutine(DashEffectCoroutine());
    }

    private void StopDashEffect()
    {
        if (_dashEffectCoroutine != null)
        {
            StopCoroutine(_dashEffectCoroutine);
            _dashEffectCoroutine = null;
        }
    }

    private IEnumerator DashEffectCoroutine()
    {
        while (_isDashing)
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