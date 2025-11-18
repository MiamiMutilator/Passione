using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lv2_Transition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public GameObject EndScreeen;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Transform root = other.transform.root;

            if (root.CompareTag("Player"))
            {
                StartCoroutine(FadeOut());
                //SceneManager.LoadScene(1);
                EndScreeen.SetActive(true);
            }


        }
    }

    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Fully opaque

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, targetColor, timer / fadeDuration);
            yield return null;
        }
    }
}
