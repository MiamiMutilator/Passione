using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Analytics;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PunchHandler : MonoBehaviour
{
    #region Variables
    #region Left Arm
    [Header("Left Jab")]
    public InputActionReference leftJabAction;
    public int leftDamage = 1;
    public Collider leftHitbox;
    public float leftHitboxDuration = 1f; // how long the hitbox lasts after activation
    public Animator leftAnimator;
    [StringPicker(options = new string[] { "EnemyHead", "EnemyBody" })]
    public string leftWeakpointTag; // Tag of the weakpoint hurtbox
    private Punch leftJab;
    #endregion
    #region Right Arm
    [Header("Right Hook")]
    public InputActionReference rightHookAction;
    public int rightDamage = 3;
    public Collider rightHitbox;
    public float rightHitboxDuration = 1f; // how long the hitbox lasts after activation
    public Animator rightAnimator;
    [StringPicker(options = new string[] { "EnemyHead", "EnemyBody" })]
    public string rightWeakpointTag; // Tag of the weakpoint hurtbox
    private Punch rightHook;
    #endregion
    [Header("General")]
    public float weakpointMult = 2f; // Damage increase when hitting a certain hurtbox
    public LayerMask targetLayer; // Damageable layer
    public Vector3 initialKnockback = Vector3.back;
    [Tooltip("The amount of time after a punch before the combo resets to 0")]
    public float comboResetTimer = 0.5f;
    public int maxCombo = 3;

    private bool isPunching;
    [HideInInspector] public int combo = 0;
    private bool timerActive;
    private float currentComboTimer;
    private Animator anim;
    #endregion

    private void OnEnable()
    {
        leftJabAction.action.Enable();
        rightHookAction.action.Enable();
    }
    private void OnDisable()
    {
        leftJabAction.action.Disable();
        rightHookAction.action.Enable();
    }

    private void Start()
    {
        leftJab = new LeftJab(gameObject, leftDamage, leftAnimator);
        rightHook = new RightHook(gameObject, rightDamage, rightAnimator);

        leftHitbox.enabled = false;
        rightHitbox.enabled = false;

        anim = GetComponent<Animator>();
        if (!anim) Debug.LogWarning("Give Animator component to Player");
        else anim.SetBool("isIdle", true);
    }

    private void Update()
    {
        anim.SetBool("isIdle", !isPunching);

        if (!isPunching && leftJabAction.action.triggered)
        {
            leftJab.OnActivation(); // Punch script handles animation and successful hit logic
            isPunching = true;
            StartCoroutine(Punch(leftHitbox, leftHitboxDuration, true)); // Handle Hitbox activation
        }

        if (!isPunching && rightHookAction.action.triggered)
        {
            rightHook.OnActivation(); // Punch script handles animation and successful hit logic
            isPunching = true;
            StartCoroutine(Punch(rightHitbox, rightHitboxDuration, false)); // Handle Hitbox activation
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

        yield return new WaitForSeconds(duration);

        hitbox.enabled = false;
        isPunching = false;
    }

    IEnumerator ComboTimer()
    {
        timerActive = true;
        while ((currentComboTimer < comboResetTimer) && timerActive)
        {
            currentComboTimer += Time.deltaTime;
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
            hitboxScript.baseKnockback = initialKnockback;
            hitboxScript.attack = leftJab;
        }
        else
        {
            PunchHitbox hitboxScript = rightHitbox.gameObject.GetComponent<PunchHitbox>();
            hitboxScript.baseDamage = rightDamage + combo;
            hitboxScript.weakpointMult = weakpointMult;
            hitboxScript.targetLayer = targetLayer;
            hitboxScript.weakpointTag = rightWeakpointTag;
            hitboxScript.baseKnockback = initialKnockback;
            hitboxScript.attack = rightHook;
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
