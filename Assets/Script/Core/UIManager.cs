using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private GameObject pauseMenu;
    
    private PlayerInput playerInput;
    private InputAction menu;
    
    private bool isPaused;
    
    public bool IsPaused => isPaused;
    
    private void Awake()
    {
        playerInput = new PlayerInput();
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void OnEnable()
    {
        menu = playerInput.Menu.UI;
        menu.Enable();
        menu.performed += Pause;
        playerInput.Player.Disable();
    }

    private void OnDisable()
    {
        menu.Disable();
        menu.performed -= Pause;
        playerInput.Player.Disable();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        if (isPaused)
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
        playerInput.Player.Disable();
        playerInput.Menu.Enable();
        playerInput.Menu.UI.Disable();
    }
    
    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;
        playerInput.Player.Disable();
        playerInput.Menu.Disable(); 
        playerInput.Menu.UI.Enable();
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
}
