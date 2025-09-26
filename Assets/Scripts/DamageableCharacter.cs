using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DamageableCharacter : MonoBehaviour, IDamageable
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
                Targetable = false;
                health = 0;
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

    public virtual void Start()
    {
        Targetable = targetable;
        health = maxHealth;
    }

    public virtual void OnHit(GameObject source, int damage)
    {
        Health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
    }
}
