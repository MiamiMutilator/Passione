using UnityEngine;

public class Blocking : MonoBehaviour
{
    public bool isBlockingHead;
    public bool isBlockingBody;

    private Renderer rend;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlockingBody)
        {
            rend.material.color = Color.red;
        }
        if (isBlockingHead)
        {
            rend.material.color = Color.blue;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BodyJab"))
        {
            if (isBlockingBody == true)
            {
                //no damage
            }
            else
            {
                //normal damage
            }
        }
        if (collision.gameObject.CompareTag("HeadHook"))
        {
            if (isBlockingHead)
            {
                //no damage
            }
            else
            {
                //normal damage
            }
        }

    }
}
