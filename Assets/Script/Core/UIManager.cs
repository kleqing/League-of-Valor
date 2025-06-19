using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
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
    
    private PlayerInput _playerInput;
    private InputAction _menu;
    private HealthBar _healthBar;
    
    private bool _isPaused;
    private int _currentRevives;
    
    public bool IsPaused => _isPaused;
    
    private void Awake()
    {
        _playerInput = new PlayerInput();
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
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
        _playerInput.Player.Disable();
        _playerInput.Menu.Enable();
        _playerInput.Menu.UI.Disable();
    }
    
    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
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
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        if (gameOverMenu != null)
        {
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
            gameOverMenu.SetActive(true);
        }
    }
    
    public void UpdateReviveText()
    {
        if (remainingReviveText != null)
        {
            remainingReviveText.text = $"{_currentRevives} remaining";
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
