using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;
    [SerializeField] private Color trailColor;

    private float targetSpeedCompensation = .1f;
    [SerializeField] private float maxFlyDistance = 15;
    private float speed = 10;
    private float damage = 1;
    private float lifeTime;
    
    private void Start()
    {
        lifeTime = maxFlyDistance / speed;
        Destroy(gameObject, lifeTime);

        Collider[] initialColliders = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialColliders.Length > 0)
        {
            OnHitObject(initialColliders[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    public void SetSpeed(float f_newSpeed)
    {
        speed = f_newSpeed;
    }

    private void CheckCollisions(float f_distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, f_distance + targetSpeedCompensation, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    private void OnHitObject(Collider f_col, Vector3 hitPoint)
    {
        IDamageable f_damageableObject = f_col.GetComponent<IDamageable>();
        if (f_damageableObject != null)
        {
            f_damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
