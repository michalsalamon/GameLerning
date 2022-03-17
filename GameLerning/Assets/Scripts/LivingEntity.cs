using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float startingHealth = 4;
    protected float health;
    protected bool dead = false;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float f_damage, RaycastHit f_hit)
    {
        health -= f_damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    private void Die()
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
