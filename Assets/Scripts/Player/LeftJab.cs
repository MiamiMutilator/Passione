using UnityEngine;

public class LeftJab : Punch
{
    public LeftJab(GameObject originator, int damage, Animator armAnimator) : base(originator, damage, armAnimator)
    {
        Originator = originator;
        Damage = damage;
        this.armAnimator = armAnimator;
    }

    public override void OnActivation()
    {
        base.OnActivation();

        // punch animation
    }
}
