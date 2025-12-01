using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerHealth))]
public class PunchHandler : ToggleableBehaviour
{
    #region Variables
    public List<PunchSettings> punchSettings;
    public float weakpointMult = 2f; // Damage increase when hitting a certain hurtbox
    public LayerMask targetLayer; // Damageable layer
    [Tooltip("The amount of time after a punch before the combo resets to 0")]
    public float comboResetTimer = 0.5f;
    public int maxCombo = 3;
    public Animator armAnimator;

    private bool isPunching;
    PlayerHealth playerHealth;
    private float baseAnimatorSpeed;
    [HideInInspector] public int combo = 0;
    private bool timerActive;
    private float currentComboTimer;
    private PlayerController controller;
    #endregion

    private void OnEnable()
    {
        foreach (var punch in punchSettings)
        {
            punch.punchInput.action.Enable();
        }
    }
    private void OnDisable()
    {
        foreach (var punch in punchSettings)
        {
            punch.punchInput.action.Disable();
        }
    }

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();

        InitializePunches();
        playerHealth.OnDamaged += CancelPunch;

        if (!armAnimator) Debug.LogError("Arm Animator is null");
        else
        {
            armAnimator.SetBool("isIdle", true);
            baseAnimatorSpeed = 1;
        }
    }

    private void Update()
    {
        AdjustAnimator();
        if (!controller.dashing && !IsPunching()) CheckPunching();
    }

    void InitializePunches()
    {
        for (int i = 0; i < punchSettings.Count; i++)
        {
            punchSettings[i].hitbox.enabled = false;
            punchSettings[i].punch = new Punch(gameObject, punchSettings[i], armAnimator);
        }
    }

    void AdjustAnimator()
    {
        if (armAnimator == null) return;

        armAnimator.SetBool("isIdle", !isPunching);
        armAnimator.speed = baseAnimatorSpeed * controller.TimeScale;
    }

    void CancelPunch()
    {
        if (isPunching)
        {
            isPunching = false;
            armAnimator.SetBool("isIdle", true);
            Debug.Log("Punch canceled");
        }
    }
 
    void CheckPunching()
    {
        // Trigger a punch based on the input
        // Get the true duration of a punch by multiplying the animation length by the reciprocal of the animation's speed multiplier, then add the endlag
        foreach (var punchSetting in punchSettings)
        {
            if (!IsPunching() && punchSetting.punchInput.action.triggered) 
            {
                isPunching = true;
                punchSetting.punch.OnActivation(); // Punch class handles animation and successful hit logic
                float duration = punchSetting.animationClip.length * (1 / armAnimator.GetFloat(punchSetting.animationLengthMultiplierName))
                                 + punchSetting.endlag;

                StartCoroutine(Punch(punchSetting, duration)); // Handle Hitbox activation
            }
        }
    }

    IEnumerator Punch(PunchSettings settings, float duration)
    {
        InitializeHitbox(settings);

        if (!timerActive)
        {
            currentComboTimer = 0;
            StartCoroutine(ComboTimer());
        }

        yield return new WaitForSeconds(duration / controller.TimeScale);

        isPunching = false;
    }

    IEnumerator ComboTimer()
    {
        timerActive = true;
        while ((currentComboTimer < comboResetTimer) && timerActive)
        {
            currentComboTimer += Time.deltaTime * controller.TimeScale;
            yield return null;
        }

        timerActive = false;
        ResetCombo();
    }

    void InitializeHitbox(PunchSettings punchSettings)
    {
        if (punchSettings.hitbox.gameObject.TryGetComponent<PunchHitbox>(out var hitbox))
        {
            hitbox.Initialize(punchSettings, weakpointMult, targetLayer);
        }
    }

    public void IncrementCombo()
    {
        combo++;
        if(combo > maxCombo) combo = maxCombo;
    }

    public void ResetCombo()
    {
        combo = 0;
        currentComboTimer = 0;
    }

    public void RegisterHit()
    {
        if (!timerActive)
        {
            StartCoroutine(ComboTimer());
        }

        if (currentComboTimer < comboResetTimer)
        {
            currentComboTimer = 0;
            IncrementCombo();
        }
        else
        {
            ResetCombo();
        }
    }

    public bool IsPunching() => isPunching;
}
