using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuSFXAndLoad : MonoBehaviour
{
    public AudioSource src;
    public AudioClip clickSfx;
    public AudioClip backSfx;
    public float sceneLoadDelay = .5f; //Loading Scene Delay

        [SerializeField] private GameObject firstSelectedButton;
    private void OnEnable()
    {
        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void PlayClickAndLoad(string sceneName)
    {
        StartCoroutine(PlayThenLoad(clickSfx, sceneName));
    }

    public void PlayBackAndLoad(string sceneName)
    {
        StartCoroutine(PlayThenLoad(backSfx, sceneName));
    }

    private IEnumerator PlayThenLoad(AudioClip clip, string sceneName)
    {
        if (clip != null)
            src.PlayOneShot(clip);

        yield return new WaitForSecondsRealtime(sceneLoadDelay);
        SceneManager.LoadScene(sceneName);
    }

    public void PlayClick() { if (clickSfx) src.PlayOneShot(clickSfx); }
    public void PlayBack() { if (backSfx) src.PlayOneShot(backSfx); }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}
