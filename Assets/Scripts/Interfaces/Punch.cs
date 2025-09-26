using UnityEngine;

public class Punch : IAttack, IActivateable
{
    public int Damage 
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
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
    private int damage;

    public Punch(GameObject originator, int damage, Animator armAnimator)
    {
        Originator = originator;
        Damage = damage;
        this.armAnimator = armAnimator;
    }

    public virtual void OnActivation()
    {
        Debug.Log(this + " activated.");
    }

    public virtual void OnSuccessfulHit()
    {
        Debug.Log("Hit successfully.");
    }
}
