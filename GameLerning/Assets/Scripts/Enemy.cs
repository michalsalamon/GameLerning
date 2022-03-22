using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]

public class Enemy : LivingEntity
{
    private enum State {Idle, Chasing, Attacing };
    private State currentState;

    [SerializeField]private ParticleSystem deathEffect;

    private NavMeshAgent pathfinder;
    private Transform target;
    private LivingEntity targetEntity;
    private Material skinMaterial;
    private Color originalColor;

    private float attackDistanceThreshold = .5f;
    private float timeBetweenAttacks = 1;
    private float damage = 1;

    private float nextAttackTime;
    private float myCollisionRadius;
    private float targetCollisionRadius;

    private bool hasTarget;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            hasTarget = true;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;
            skinMaterial = GetComponent<Renderer>().material;
            originalColor = skinMaterial.color;

            currentState = State.Chasing;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    private void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    IEnumerator Attack()
    {
        currentState = State.Attacing;
        pathfinder.enabled = false;

        Vector3 f_originalPosition = transform.position;
        Vector3 f_directionToTarget = (target.position - transform.position).normalized;
        Vector3 f_attackPosition = target.position - f_directionToTarget * (myCollisionRadius);

        float f_percent = 0;
        float f_attackSpeed = 3;

        bool f_hasAppliedDamage = false;

        skinMaterial.color = Color.blue;

        while (f_percent <= 1)
        {
            if (f_percent >= .5f && !f_hasAppliedDamage)
            {
                f_hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            f_percent += Time.deltaTime * f_attackSpeed;
            float f_interpolation = (-Mathf.Pow(f_percent, 2) + f_percent) * 4;
            transform.position = Vector3.Lerp(f_originalPosition, f_attackPosition, f_interpolation);

            yield return null;
        }

        currentState = State.Chasing;
        pathfinder.enabled = true;
        skinMaterial.color = originalColor;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 f_directionToTarget = (target.position - transform.position).normalized;
                Vector3 f_targetPosition = target.position - f_directionToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathfinder.SetDestination(f_targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public override void TakeHit(float f_damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (f_damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), deathEffect.main.startLifetimeMultiplier);
        }
        base.TakeHit(f_damage, hitPoint, hitDirection);
    }
}
