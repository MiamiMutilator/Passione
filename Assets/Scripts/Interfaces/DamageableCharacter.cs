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

    private Rigidbody rb;

    public virtual void Start()
    {
        Targetable = targetable;
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogWarning("Put Rigidbody on " + gameObject.name);
    }

    public virtual void OnHit(IAttack source, int damage)
    {
        Health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage from " + source.Originator + ". " + health + " health remaining.");
    }

    public virtual void OnHitWithKnockback(int damage, Vector3 knockback)
    {
        Health -= damage;
        if (rb)
        {
            rb.AddForce(knockback, ForceMode.Impulse);
        }
        Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
    }
}
