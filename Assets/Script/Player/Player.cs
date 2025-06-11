using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")] [SerializeField]
    private float speed;

    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movement;

    private PlayerInput playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = new PlayerInput();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
        playerInput.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movement = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            movement = Vector2.zero;
        }
    }

    private void Update()
    {
        anim.SetBool("isRun", movement != Vector2.zero);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * speed, movement.y * speed);
    }
}