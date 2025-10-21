using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSFXAndLoad : MonoBehaviour
{
    public AudioSource src;
    public AudioClip clickSfx;
    public AudioClip backSfx;
    public float sceneLoadDelay = .5f; // Loading Scene Delay

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

        // Wait timer before loading scene
        yield return new WaitForSecondsRealtime(sceneLoadDelay);

        SceneManager.LoadScene(sceneName);
    }

    // Sound for regular buttons
    public void PlayClick() { if (clickSfx) src.PlayOneShot(clickSfx); }
    public void PlayBack()  { if (backSfx)  src.PlayOneShot(backSfx); }
}

