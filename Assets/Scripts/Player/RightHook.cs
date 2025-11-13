using UnityEngine;

public class RightHook : Punch
{
    public RightHook(GameObject originator, Animator armAnimator) : base(originator, armAnimator)
    {
        Originator = originator;
        this.armAnimator = armAnimator;
    }

    public override void OnActivation()
    {
        base.OnActivation();

        if (armAnimator)
        {
            armAnimator.SetTrigger("RightPunch");
        }
    }
}
