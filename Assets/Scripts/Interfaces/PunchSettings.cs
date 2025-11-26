using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PunchSettings
{
    [Tooltip("\"Right Hook\", \"Left Jab\", etc")]
    public string type;
    public AnimationClip animationClip;
    [Tooltip("\"RightHook\", \"LeftJab\", etc")]
    public string animatorTrigger;
    public InputActionReference punchInput;
    public int damage;
    [Tooltip("How much the Rage Meter increases by on a successful hit")]
    public int rageIncrease = 20;
    public float knockbackStrength;
    public Collider hitbox;
    [Tooltip("Tag of the weakpoint hurtbox")]
    public string weakpointTag; // Tag of the weakpoint hurtbox
    [Tooltip("Tag of the blocking hurtbox")]
    public string blockTag; // Tag of the enemy blocking hurtbox
    [Tooltip("\"RightMult\", \"LeftMult\", etc")]
    public string animationLengthMultiplierName;
    public float endlag;

    public Punch punch;
}
