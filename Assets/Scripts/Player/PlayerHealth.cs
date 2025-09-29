using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int Health
    {
        set
        {
            if (value > 0 && value < health)
            {
                // Hit animation and sound
            }

            health = value;

            if (health > maxHealth)
            {
                health = maxHealth;
            }

            if (health <= 0 && Targetable)
            {
                targetable = false;
                health = 0;
                Debug.Log(gameObject.name + " health depleted.");
            }
        }
        get
        {
            return health;
        }
    }
    public bool Targetable { get; set; }

    public int maxHealth = 10;
    public int health = 10;
    public bool targetable = true;
    public UnityEvent OnDestroyEvents;

    public void Start()
    {
        Targetable = targetable;
        health = maxHealth;
    }

    public void OnHit(IAttack source, int damage)
    {
        Health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage from " + source.Originator + ". " + health + " health remaining.");
    }
}
