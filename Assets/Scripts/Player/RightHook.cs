using UnityEngine;

public class RightHook : Punch
{
    public RightHook(GameObject originator, int damage, Animator armAnimator) : base(originator, damage, armAnimator)
    {
        Originator = originator;
        Damage = damage;
        this.armAnimator = armAnimator;
    }

    public override void OnActivation()
    {
        base.OnActivation();

        Animator anim = Originator.GetComponent<Animator>();

        if (anim)
        {
            anim.SetTrigger("RightPunch");
        }
    }
}
