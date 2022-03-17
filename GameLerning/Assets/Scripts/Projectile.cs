using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;
    private float maxFlyDistance = 15;
    private float speed = 10;
    private float damage = 1;

    // Update is called once per frame
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);

        maxFlyDistance -= moveDistance;
        if (maxFlyDistance <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float f_newSpeed)
    {
        speed = f_newSpeed;
    }

    private void CheckCollisions(float f_distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, f_distance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    private void OnHitObject(RaycastHit f_hit)
    {
        IDamageable f_damageableObject = f_hit.collider.GetComponent<IDamageable>();
        if (f_damageableObject != null)
        {
            f_damageableObject.TakeHit(damage, f_hit);
        }
        GameObject.Destroy(gameObject);
    }
}
