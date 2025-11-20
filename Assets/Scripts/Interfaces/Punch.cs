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

    public Animator armAnimator;
    private GameObject originator;
    protected PunchHandler handler;
    protected float startTime;

    public Punch(GameObject originator, Animator armAnimator)
    {
        Originator = originator;
        this.armAnimator = armAnimator;

        handler = Originator.GetComponent<PunchHandler>();
    }

    public virtual void OnActivation()
    {
        Debug.Log(this + " activated.");
        startTime = Time.time;
    }

    public virtual void OnSuccessfulHit()
    {
        Debug.Log("Hit successfully.");

        handler.RegisterHit();

        // logic for increasing rage
        // anything else happening when a punch connects
    }
}
