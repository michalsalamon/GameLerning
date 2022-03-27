using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float startingHealth = 4;
    public float StartingHealth
    { get { return startingHealth; } }
    public float health { get; protected set; }
    protected bool dead = false;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float f_damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(f_damage);
    }

    public virtual void TakeDamage(float f_damage)
    {
        health -= f_damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destroy")]
    public virtual void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }

    public event System.Action OnDeath;
}
