using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private GameObject pauseMenu;
    
    [Header("Revive Settings")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Text remainingReviveText;
    [SerializeField] private int maxRevives;
    
    [Header("Menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject shop;

    [Header("Text")]
    [SerializeField] private Text rangeButtonText;
    [SerializeField] private Text meleeButtonText;

    private PlayerInput _playerInput;
    private InputAction _menu;
    private HealthBar _healthBar;
    
    private bool _isPaused;
    private int _currentRevives;
    
    public bool IsPaused => _isPaused;
    private bool isMeleeUnlocked = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        if (shop != null)
        {
            shop.SetActive(false);
        }
        
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }

        _currentRevives = maxRevives;
        
        var player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            _healthBar = player.GetComponent<HealthBar>();
        }
        
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(LoadSelectedWeapon());
        UpdateReviveText();
    }

    private void OnEnable()
    {
        _menu = _playerInput.Menu.UI;
        _menu.Enable();
        _menu.performed += Pause;
        _playerInput.Player.Disable();
    }

    private void OnDisable()
    {
        _menu.Disable();
        _menu.performed -= Pause;
        _playerInput.Player.Disable();
    }

    public void OnConfirmPaymentClicked()
    {
        var payment = FindAnyObjectByType<PaymentManagement>();
        StartCoroutine(payment.CheckPaymentStatus("kleqing"));
    }

    public void Pause(InputAction.CallbackContext context)
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    public void ActivateMenu()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        mainMenu.SetActive(false);
        _playerInput.Player.Disable();
        _playerInput.Menu.Enable();
        _playerInput.Menu.UI.Disable();
    }
    
    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        mainMenu.SetActive(false);
        _isPaused = false;
        _playerInput.Player.Disable();
        _playerInput.Menu.Disable(); 
        _playerInput.Menu.UI.Enable();
    }

    public void Quit()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    
    public void RestartGame()
    {
        mainMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        if (gameOverMenu != null)
        {
            mainMenu.SetActive(false);
            gameOverMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Revive()
    {
        if (_healthBar == null)
        {
            return;
        }
        if (_currentRevives > 0)
        {
            _currentRevives--;
            _healthBar.Revive();
            UpdateReviveText();
            
            if (gameOverMenu != null)
            {
                gameOverMenu.SetActive(false);
            }

            Time.timeScale = 1f;

            _playerInput.Player.Enable();
            _playerInput.Menu.Disable();
            _playerInput.Menu.UI.Enable();
        }
        else
        {
            var payment = FindAnyObjectByType<PaymentManagement>();
            if (payment != null)
            {
                Debug.Log("Trigger PayOS...");
                payment.BuyRevive();
            }
            else
            {
                Debug.LogWarning("PaymentManagement not found");
            }
        }
    }
    
    public void UpdateReviveText()
    {
        mainMenu.SetActive(false);
        if (_currentRevives == 0)
        {
            remainingReviveText.text = "Want more? Buy a revive!";
        }
        else
        {
            remainingReviveText.text = $"{_currentRevives} remaining";
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        mainMenu.SetActive(false);
    }

    private void CloseGameOverUI()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }

        Time.timeScale = 1f;
        _playerInput.Player.Enable();
        _playerInput.Menu.Disable();
        _playerInput.Menu.UI.Enable();
    }

    public void OpenShop()
    {
        mainMenu.SetActive(false);
        shop.SetActive(true);
    }

    public void BackToMenu()
    {
        shop.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void SelectRange()
    {
        Debug.Log("Range choosen");
        GameManager.Instance.selectedWeapon = GameManager.WeaponType.Range;
        UpdateWeaponButtons();
    }

    public void TrySelectMelee()
    {
        var payment = FindAnyObjectByType<PaymentManagement>();
        StartCoroutine(payment.CheckWeaponUnlocked("melee", (isUnlocked) =>
        {
            if (isUnlocked)
            {
                Debug.Log("Melee choosen");
                GameManager.Instance.selectedWeapon = GameManager.WeaponType.Melee;
                UpdateWeaponButtons();
            }
            else
            {
                Debug.Log("Please pay to unlock this weapon...");
                payment.BuyMeleeWeapon();
            }
        }));
    }

    private void UpdateWeaponButtons()
    {
        if (GameManager.Instance.selectedWeapon == GameManager.WeaponType.Melee)
        {
            meleeButtonText.text = "Selected";
            rangeButtonText.text = "Select";
        }
        else
        {
            meleeButtonText.text = isMeleeUnlocked ? "Select" : "Select";
            rangeButtonText.text = "Selected";
        }
    }

    private IEnumerator LoadSelectedWeapon()
    {
        string url = $"https://localhost:7028/api/payment/unlocked-weapon?playerName=kleqing";
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            var result = JsonUtility.FromJson<WeaponResponse>(json);

            if (result.weapon == "Melee")
                GameManager.Instance.selectedWeapon = GameManager.WeaponType.Melee;
            else
                GameManager.Instance.selectedWeapon = GameManager.WeaponType.Range;

            UpdateWeaponButtons();
        }
        else
        {
            Debug.LogError("LoadSelectedWeapon error: " + req.error);
        }
    }

    [System.Serializable]
    private class WeaponResponse
    {
        public string weapon;
    }

}
