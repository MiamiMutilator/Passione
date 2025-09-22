using UnityEngine;
using System.Collections;

public class Rage : MonoBehaviour
{
    public int RageMeter = 100;
    public bool enraged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && RageMeter == 100)
        {
            Debug.Log("Enraged!");
            enraged = true;
            StartCoroutine(RageDown());
        }
        if (RageMeter == 0)
        {
            enraged = false;
            Debug.Log("No longer enraged");
            StopCoroutine(RageDown());
            StartCoroutine(RageUp());
        }
    }

    private IEnumerator RageDown()
    {
        while (enraged == true && RageMeter > 0)
        {
            RageMeter--;
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator RageUp()
    {
        while (enraged == false && RageMeter < 100f)
        {
            RageMeter++;
            yield return new WaitForSeconds(0.5f);
        }
    }

}
