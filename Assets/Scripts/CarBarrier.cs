using System.Collections;
using UnityEngine;

public class CarBarrier : MonoBehaviour
{
    public Rigidbody Car1, Car2, Car3, Car4, Car5, Car6, Car7, Car8;
    public GameObject C1, C2, C3, C4, C5, C6, C7, C8;
    public Transform Player;
    public float PushBackForce = 50f;

    public float moveSpeed = 50f;
    public float movementDuration = 5f;

    private Coroutine carMovementCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
                Vector3 currentPosition = root.position;
                currentPosition.z -= PushBackForce;
                root.GetComponent<PlayerController>()?.ApplyKnockback(-root.forward * PushBackForce);

                if (carMovementCoroutine == null) // only start if not already running
                {
                    carMovementCoroutine = StartCoroutine(MoveCarsForSeconds());
                }
            }

            Debug.Log("Player hit wall");

        }
    }


    IEnumerator MoveCarsForSeconds()
    {
        C1.SetActive(true);
        C2.SetActive(true);
        C3.SetActive(true);
        C4.SetActive(true);
        C5.SetActive(true);
        C6.SetActive(true);
        C7.SetActive(true);
        C8.SetActive(true);
        float elapsed = 0f;

        // Move right for the whole duration
        while (elapsed < movementDuration/2)
        {
            Vector3 v = Vector3.left * moveSpeed;
            Vector3 b = Vector3.right * moveSpeed;

            Car1.linearVelocity = v;
            Car2.linearVelocity = b;
            Car3.linearVelocity = v;
            Car4.linearVelocity = b;
            Car5.linearVelocity = v;
            Car6.linearVelocity = b;
            Car7.linearVelocity = v;
            Car8.linearVelocity = b;

            elapsed += Time.deltaTime;
            yield return null;
        }

        while (elapsed > movementDuration/2 && elapsed < movementDuration)
        {
            Vector3 v = Vector3.right * moveSpeed;
            Vector3 b = Vector3.left * moveSpeed;

            Car1.linearVelocity = v;
            Car2.linearVelocity = b;
            Car3.linearVelocity = v;
            Car4.linearVelocity = b;
            Car5.linearVelocity = v;
            Car6.linearVelocity = b;
            Car7.linearVelocity = v;
            Car8.linearVelocity = b;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop movement
        C1.SetActive(false);
        C2.SetActive(false);
        C3.SetActive(false);
        C4.SetActive(false);
        C5.SetActive(false);
        C6.SetActive(false);
        C7.SetActive(false);
        C8.SetActive(false);

        Car1.linearVelocity = Vector3.zero;
        Car2.linearVelocity = Vector3.zero;
        Car3.linearVelocity = Vector3.zero;
        Car4.linearVelocity = Vector3.zero;
        Car5.linearVelocity = Vector3.zero;
        Car6.linearVelocity = Vector3.zero;
        Car7.linearVelocity = Vector3.zero;
        Car8.linearVelocity = Vector3.zero;

        carMovementCoroutine = null;
    }


}
