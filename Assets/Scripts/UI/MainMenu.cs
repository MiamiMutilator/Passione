using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("UI Test");
    }
}
