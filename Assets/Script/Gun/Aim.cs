using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject bullets;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform player;
    [Range(0f, 2f)] [SerializeField] private float fireRate;

    private float fireTimer;
    private Vector2 worldPosition;
    private Vector2 direction;
    private float angle;
    private PlayerInput playerInput;
    private bool isFiring;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Attack.performed += OnFireStart;
        playerInput.Player.Attack.canceled += OnFireStop;
    }

    private void OnDisable()
    {
        playerInput.Player.Attack.performed -= OnFireStart;
        playerInput.Player.Attack.canceled -= OnFireStop;
        playerInput.Disable();
    }

    private void Update()
    {
        HandleGunRotation();
        fireTimer -= Time.deltaTime;
        
        if (fireTimer <= 0f)
        {
            //* Spamming bullets
            Instantiate(bullets, bulletSpawnPoint.position, gun.transform.rotation);
            fireTimer = fireRate;
        }
    }

    private void HandleGunRotation()
    {
        worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        direction = (worldPosition - (Vector2)gun.transform.position).normalized;

        //* Fix the gun rotation based on the player's direction
        float playerDirection = Mathf.Sign(player.localScale.x);
        gun.transform.localScale = new Vector3(playerDirection, player.localScale.y, player.localScale.z);

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //* Fix the gun projectile rotation based on the player's direction
        if (angle > 90 || angle < -90)
        {
            gun.transform.rotation = Quaternion.Euler(180, 0, -angle);
        }
        else
        {
            gun.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnFireStart(InputAction.CallbackContext context)
    {
        isFiring = true;
    }

    private void OnFireStop(InputAction.CallbackContext context)
    {
        isFiring = false;
    }
}