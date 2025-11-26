using UnityEngine;

public class Punch : IAttack, IActivateable
{
    public GameObject Originator
    {
        get
        {
            return originator;
        }
        set
        {
            originator = value;
        }
    }
    public PunchSettings settings;
    public Animator armAnimator;
    private GameObject originator;
    protected PunchHandler handler;
    protected float startTime;

    public Punch(GameObject originator, PunchSettings settings, Animator armAnimator)
    {
        Originator = originator;
        this.settings = settings;
        this.armAnimator = armAnimator;

        handler = Originator.GetComponent<PunchHandler>();
    }

    public virtual void OnActivation()
    {
        Debug.Log(settings.type + " activated");
        startTime = Time.time;

        if (armAnimator)
        {
            armAnimator.SetTrigger(settings.animatorTrigger);
        }
    }

    public virtual void OnSuccessfulHit()
    {
        Debug.Log("Hit successfully");

        handler.RegisterHit();

        // logic for increasing rage
        if (originator.TryGetComponent<Rage>(out var rage))
        {
            if (!rage.enraged)
                rage.IncreaseRage(settings.rageIncrease);
        }
        // anything else happening when a punch connects
    }
}
