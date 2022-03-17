using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]

public class Enemy : LivingEntity
{
    private enum State {Idle, Chasing, Attacing };
    private State currentState;

    private NavMeshAgent pathfinder;
    private Transform target;
    private Material skinMaterial;
    private Color originalColor;

    private float attackDistanceThreshold = .5f;
    private float timeBetweenAttacks = 1;

    private float nextAttackTime;
    private float myCollisionRadius;
    private float targetCollisionRadius;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        currentState = State.Chasing;

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
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

    IEnumerator Attack()
    {
        currentState = State.Attacing;
        pathfinder.enabled = false;

        Vector3 f_originalPosition = transform.position;
        Vector3 f_directionToTarget = (target.position - transform.position).normalized;
        Vector3 f_attackPosition = target.position - f_directionToTarget * (myCollisionRadius);

        float f_percent = 0;
        float f_attackSpeed = 3;

        skinMaterial.color = Color.blue;

        while (f_percent <= 1)
        {
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
        while (target != null)
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
}
