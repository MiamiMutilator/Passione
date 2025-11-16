using UnityEngine.EventSystems;
using UnityEngine;

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
            playerToggleControl.Toggle(true); //toggle

        if (EventSystem.current)
            EventSystem.current.SetSelectedGameObject(null);
    }

}
