using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int Health
    {
        set
        {
            if (value > 0 && value < health)
            {
                remainingShake = shakeDuration;
            }

            health = value;

            if (health > maxHealth)
            {
                health = maxHealth;
            }

            if (health <= 0 && Targetable)
            {
                targetable = false;
                health = 0;
                Debug.Log(gameObject.name + " health depleted.");
            }
        }
        get
        {
            return health;
        }
    }
    public bool Targetable { get; set; }

    public int maxHealth = 10;
    public int health = 10;
    public bool targetable = true;
    public UnityEvent OnDestroyEvents;
    [Header("Camera Shake")]
    public bool cameraShakeOn = true;
    public Camera playerCam;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
    public float shakeDuration = 0.4f;

    float remainingShake = 0;
    Vector3 originalCameraPosition;

    public void Start()
    {
        Targetable = targetable;
        health = maxHealth;
        originalCameraPosition = playerCam.transform.localPosition;
    }

    void Update()
    {
        if (cameraShakeOn) CameraShake();
    }

    void CameraShake()
    {
        if (remainingShake > 0)
        {
            playerCam.transform.localPosition = (Random.insideUnitSphere + originalCameraPosition) * shakeAmount;
            remainingShake -= Time.deltaTime * decreaseFactor;

        }
        else
        {
            playerCam.transform.localPosition = originalCameraPosition;
            remainingShake = 0f;
        }
    }

    public void OnHit(int damage)
    {
        Health -= damage;

        Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
    }
}
