using UnityEngine;

public class LeftJab : Punch
{
    public LeftJab(GameObject originator, Animator armAnimator) : base(originator, armAnimator)
    {
        Originator = originator;
        this.armAnimator = armAnimator;
    }

    public override void OnActivation()
    {
        base.OnActivation();

        if (armAnimator)
        {
            armAnimator.SetTrigger("LeftPunch");
        }
    }
}
