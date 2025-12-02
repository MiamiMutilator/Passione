using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;     // PauseMenuCanvas
    [SerializeField] private GameObject settingsMenu;  // Settings Pause

    [Header("Navigation")]
    [SerializeField] private GameObject firstSelectedSetting; // Back Button

    [Header("Input")]
    [SerializeField] private InputActionReference menuBinding; 

    private void OnEnable()
    {
        menuBinding.action.Enable();
        menuBinding.action.performed += OnMenuPressed;

        // 
        if (firstSelectedSetting != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedSetting);
        }
    }

    private void OnDisable()
    {
        menuBinding.action.Disable();
        menuBinding.action.performed -= OnMenuPressed;
    }

    // re opens
    private void OnMenuPressed(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            CloseSettings();
        }
    }

    // Calling Setting //No GameObject.SetActive
    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedSetting);
        }
    }

    // Calling Back on SettingPause
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);

        // Select pause menu first selected (Resume)
        PauseMenu pm = pauseMenu.GetComponentInParent<PauseMenu>();
        if (pm != null && pm.firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pm.firstSelectedButton);
        }
    }
}
