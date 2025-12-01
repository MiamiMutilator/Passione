using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


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
    public float smoothness = 0.5f;
    public float shakeFrequency = 15f;
    public float shakeDuration = 0.4f;

    //Blur
    [Header("Hit Blur")]
    public Volume postProcessVolume;
    private DepthOfField dof;
    public float blurDuration = 0.4f;
    private float remainingBlur = 0f;
    private bool blurring = false;


    float remainingShake = 0;
    bool isShaking = false;
    Vector3 originalCameraPosition;

    public void Start()
    {
        Targetable = targetable;
        health = maxHealth;
        originalCameraPosition = playerCam.transform.localPosition;

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out dof);
            dof.gaussianStart.value = 0f;
            dof.gaussianEnd.value = 2f;
            //dof.gaussianMaxRadius.value = 10f;
            dof.active = false;
        }
    }

    void Update()
    {
        if (cameraShakeOn) CameraShake();
        HitBlur();
    }

    void CameraShake()
    {
        if (remainingShake > 0f && !isShaking)
        {
            isShaking = true;
        }

        if (isShaking && remainingShake > 0f)
        {
            float shake = shakeAmount * (remainingShake / shakeDuration);

            float x = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * shake;
            float y = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * shake;

            Vector3 targetPos = originalCameraPosition + new Vector3(x, y, 0);

            playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, targetPos, smoothness);

            remainingShake -= Time.deltaTime;
        }
        else if (isShaking)
        {
            isShaking = false;
            remainingShake = 0f;

            playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, originalCameraPosition, smoothness);
        }
    }

    void HitBlur()
    {
        if (dof == null) return;

        if (remainingBlur > 0f && !blurring)
        {
            blurring = true;
            dof.active = true;
        }

        if (blurring && remainingBlur > 0f)
        {
            float t = remainingBlur / blurDuration;

            dof.gaussianStart.value = Mathf.Lerp(0.1f, 0.5f, t);
            dof.gaussianEnd.value = Mathf.Lerp(1f, 3f, t);
            dof.gaussianMaxRadius.value = Mathf.Lerp(0.1f, 2f, t);

            remainingBlur -= Time.deltaTime;
        }
        else if (blurring)
        {
            blurring = false;
            remainingBlur = 0f;

            dof.active = false;
        }
    }


    public void OnHit(int damage)
    {
        Health -= damage;

        remainingBlur = blurDuration;

        Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
    }
}
