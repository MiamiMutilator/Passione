using System.Collections;
using UnityEngine;

public class CarBarrier : MonoBehaviour
{
    [Header("Car References")]
    public Rigidbody Car1, Car2, Car3, Car4, Car5, Car6, Car7, Car8;
    public GameObject C1, C2, C3, C4, C5, C6, C7, C8;

    [Header("Player & Settings")]
    public Transform Player;
    public float PushBackForce = 50f;
    public float moveSpeed = 50f;
    public float movementDuration = 5f;

    private Rigidbody[] cars;
    private GameObject[] carObjects;
    private Vector3[] startPositions;
    private Vector3[] directions; // original directions for each car
    private Coroutine carMovementCoroutine;

    void Start()
    {
        // Initialize arrays
        cars = new Rigidbody[] { Car1, Car2, Car3, Car4, Car5, Car6, Car7, Car8 };
        carObjects = new GameObject[] { C1, C2, C3, C4, C5, C6, C7, C8 };

        // Store original positions
        startPositions = new Vector3[cars.Length];
        for (int i = 0; i < cars.Length; i++)
            startPositions[i] = cars[i].transform.position;

        // Store original movement directions (matching your original script)
        directions = new Vector3[]
        {
            Vector3.right,  // Car1
            Vector3.right, // Car2
            Vector3.left,  // Car3
            Vector3.right, // Car4
            Vector3.left,  // Car5
            Vector3.left, // Car6
            Vector3.left,  // Car7
            Vector3.left  // Car8
        };
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform root = other.transform.root;

            if (root.CompareTag("Player"))
            {
                // Apply knockback to player
                root.GetComponent<PlayerController>()?.ApplyKnockback(-root.forward * PushBackForce);

                // Start car movement coroutine if not already running
                if (carMovementCoroutine == null)
                    carMovementCoroutine = StartCoroutine(MoveCarsForSeconds());
            }

            Debug.Log("Player hit wall");
        }
    }

    IEnumerator MoveCarsForSeconds()
    {
        // Enable all car objects
        foreach (var obj in carObjects)
            obj.SetActive(true);

        float elapsed = 0f;

        while (elapsed < movementDuration)
        {
            // Teleport all cars back to their original positions
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i].linearVelocity = Vector3.zero;
                cars[i].angularVelocity = Vector3.zero;
                cars[i].Sleep(); // stops physics immediately

                cars[i].transform.position = startPositions[i];
            }

            yield return null; // wait a frame after teleport

            // Move cars in their original directions
            float moveTime = movementDuration / 2f;
            float t = 0f;

            while (t < moveTime)
            {
                for (int i = 0; i < cars.Length; i++)
                {
                    cars[i].linearVelocity = directions[i] * moveSpeed;
                }

                t += Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Stop cars before next teleport
            foreach (var car in cars)
                car.linearVelocity = Vector3.zero;
        }

        // Disable cars after movement finishes
        foreach (var obj in carObjects)
            obj.SetActive(false);

        // Reset velocities
        foreach (var car in cars)
            car.linearVelocity = Vector3.zero;

        carMovementCoroutine = null;
    }
}
