using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject firstSelected;

    private Animator[] pauseAnimators;
    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //animators
        pauseAnimators = pauseMenu.GetComponentsInChildren<Animator>(true);
        //animate offscreen
        foreach (var anim in pauseAnimators)
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
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

        // ui animator unscaled time
        foreach (var anim in pauseAnimators)
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;

        if (EventSystem.current && firstSelected)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void ResumeGame()
    {
        //returns animator
        foreach (var anim in pauseAnimators)
            anim.updateMode = AnimatorUpdateMode.Normal;

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (EventSystem.current)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    public void Options()
    {
        Debug.Log("Options");
    }


    public void QuitGame() { Application.Quit(); }
}