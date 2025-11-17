using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PunchHandler : ToggleableBehaviour
{
    #region Variables
    #region Left Arm
    [Header("Left Jab")]
    public InputActionReference leftJabAction;
    public int leftDamage = 1;
    public float leftKnockbackStrength = 3f;
    public Collider leftHitbox;
    [StringPicker(options = new string[] { "EnemyHead", "EnemyBody" })]
    public string leftWeakpointTag; // Tag of the weakpoint hurtbox
    public string leftBlockTag; // Tag of the enemy blocking hurtbox
    public float leftPunchEndlag = 0;

    float leftJabAnimationLength = 1f; // how long the hitbox lasts after activation
    private Punch leftJab;
    #endregion
    #region Right Arm
    [Header("Right Hook")]
    public InputActionReference rightHookAction;
    public int rightDamage = 3;
    public float rightKnockbackStrength = 10f;
    public Collider rightHitbox;
    [StringPicker(options = new string[] { "EnemyHead", "EnemyBody" })]
    public string rightWeakpointTag; // Tag of the weakpoint hurtbox
    public string rightBlockTag; // Tag of the enemy blocking hurtbox
    public float rightPunchEndlag = 0.2f;

    float rightHookAnimationLength = 1f; // how long the hitbox lasts after activation
    private Punch rightHook;
    #endregion
    [Header("General")]
    public float weakpointMult = 2f; // Damage increase when hitting a certain hurtbox
    public LayerMask targetLayer; // Damageable layer
    [Tooltip("The amount of time after a punch before the combo resets to 0")]
    public float comboResetTimer = 0.5f;
    public int maxCombo = 3;
    public Animator armAnimator;

    private bool isPunching;
    private float baseAnimatorSpeed;
    [HideInInspector] public int combo = 0;
    private bool timerActive;
    private float currentComboTimer;
    private PlayerController controller;
    
    #endregion

    private void OnEnable()
    {
        leftJabAction.action.Enable();
        rightHookAction.action.Enable();
    }
    private void OnDisable()
    {
        leftJabAction.action.Disable();
        rightHookAction.action.Disable();
    }

    private void Start()
    {
        controller = GetComponent<PlayerController>();

        leftJab = new LeftJab(gameObject, armAnimator);
        rightHook = new RightHook(gameObject, armAnimator);

        leftHitbox.enabled = false;
        rightHitbox.enabled = false;

        if (!armAnimator) Debug.LogWarning("Arm Animator is null");
        else
        {
            armAnimator.SetBool("isIdle", true);
            baseAnimatorSpeed = 1;
            UpdateAnimClipTimes();
        }
    }

    private void Update()
    {
        AdjustAnimator();
        if (!controller.dashing) CheckPunching();
    }

    void AdjustAnimator()
    {
        if (armAnimator == null) return;

        armAnimator.SetBool("isIdle", !isPunching);
        armAnimator.speed = baseAnimatorSpeed * controller.TimeScale;
    }

    void CheckPunching()
    {
        // Trigger a left or right punch based on the input
        // Get the true duration of a punch by multiplying the animation length by the reciprocal of the animation's speed multiplier, then add the endlag

        if (!IsPunching() && leftJabAction.action.triggered)
        {
            leftJab.OnActivation(); // Punch script handles animation and successful hit logic
            isPunching = true;
            StartCoroutine(Punch(leftHitbox, leftJabAnimationLength * (1 / armAnimator.GetFloat("LeftMult")) + leftPunchEndlag, true)); // Handle Hitbox activation
        }

        if (!IsPunching() && rightHookAction.action.triggered)
        {
            rightHook.OnActivation(); // Punch script handles animation and successful hit logic
            isPunching = true;
            StartCoroutine(Punch(rightHitbox, rightHookAnimationLength * (1 / armAnimator.GetFloat("RightMult")) + rightPunchEndlag, false)); // Handle Hitbox activation
        }
    }

    IEnumerator Punch(Collider hitbox, float duration, bool isLeft)
    {
        InitializeHitbox(isLeft);
        hitbox.enabled = true;

        if (!timerActive)
        {
            currentComboTimer = 0;
            StartCoroutine(ComboTimer());
        }

        yield return new WaitForSeconds(duration / controller.TimeScale);

        hitbox.enabled = false;
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

    void InitializeHitbox(bool isLeft)
    {
        if (isLeft)
        {
            PunchHitbox hitboxScript = leftHitbox.gameObject.GetComponent<PunchHitbox>();
            hitboxScript.baseDamage = leftDamage + combo;
            hitboxScript.weakpointMult = weakpointMult;
            hitboxScript.targetLayer = targetLayer;
            hitboxScript.weakpointTag = leftWeakpointTag;
            hitboxScript.blockedTag = leftBlockTag;
            hitboxScript.attack = leftJab;
            hitboxScript.knockbackFactor = leftKnockbackStrength;
        }
        else
        {
            PunchHitbox hitboxScript = rightHitbox.gameObject.GetComponent<PunchHitbox>();
            hitboxScript.baseDamage = rightDamage + combo;
            hitboxScript.weakpointMult = weakpointMult;
            hitboxScript.targetLayer = targetLayer;
            hitboxScript.weakpointTag = rightWeakpointTag;
            hitboxScript.blockedTag = rightBlockTag;
            hitboxScript.attack = rightHook;
            hitboxScript.knockbackFactor = rightKnockbackStrength;
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

    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = armAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Left Punch":
                    leftJabAnimationLength = clip.length;
                    break;
                case "Right Punch":
                    rightHookAnimationLength = clip.length;
                    break;
            }
        }
    }

    public bool IsPunching() => isPunching;
}

#region Custom Editor
public class StringPickerAttribute : PropertyAttribute
{
    public string[] options;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringPickerAttribute))]
public class StringPickerAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (StringPickerAttribute)attribute;
        EditorGUI.BeginProperty(position, label, property);

        var propertyRect = new Rect(position.x, position.y, position.width - 20, position.height);
        var dropdownButtonRect = new Rect(propertyRect.xMax, position.y, 20, position.height);

        EditorGUI.PropertyField(propertyRect, property);

        if (GUI.Button(dropdownButtonRect, "Next Move"))
        {
            var menu = new GenericMenu();
            foreach (var option in attr.options)
            {
                menu.AddItem(new GUIContent(option.ToString()), false,
                    () =>
                    {
                        // set the property value to selected
                        property.stringValue = option;
                        // Apply the modified values
                        property.serializedObject.ApplyModifiedProperties();
                    });
            }
            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }
}
#endif

#endregion
