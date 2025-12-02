using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneToGame : MonoBehaviour
{
    void Start()
    {
        PlayableDirector director = GetComponent<PlayableDirector>();
        director.stopped += LoadNextScene;
    }

    void LoadNextScene(PlayableDirector director)
    {
        SceneManager.LoadScene("Lvl_1_Art");
    }
}