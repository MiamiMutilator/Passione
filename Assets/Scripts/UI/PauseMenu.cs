using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference menuBinding;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private ToggleControl playerToggleControl; // drag Player
    [SerializeField] public GameObject firstSelectedButton;    // drag Resume Button
    public static bool isPaused;

    private void OnEnable()
    {
        menuBinding.action.Enable();
        menuBinding.action.performed += OnMenuPressed;

        
        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    private void OnDisable()
    {
        menuBinding.action.Disable();
        menuBinding.action.performed -= OnMenuPressed;
    }

    void Start()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnMenuPressed(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerToggleControl)
            playerToggleControl.Toggle(false);

        
        if (EventSystem.current)
            EventSystem.current.SetSelectedGameObject(null);

        
        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerToggleControl)
            playerToggleControl.Toggle(true);

        if (EventSystem.current)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (playerToggleControl)
            playerToggleControl.Toggle(true);

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
