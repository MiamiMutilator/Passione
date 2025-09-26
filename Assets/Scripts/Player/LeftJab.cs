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

    public override void OnSuccessfulHit()
    {
        base.OnSuccessfulHit();

        // logic for increasing rage
        // anything else happening when a punch connects
    }
}
