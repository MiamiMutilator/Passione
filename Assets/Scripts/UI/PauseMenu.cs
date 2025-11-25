using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private ToggleControl playerToggleControl; //drag Player
    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
