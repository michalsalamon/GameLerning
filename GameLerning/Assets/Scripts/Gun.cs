using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float msBetweenShot = 100;
    [SerializeField] private float muzzleVelocity = 35;

    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjector;
    private MuzzleFlash muzzleFlash;

    private float nextShotTime;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShot / 1000;
            Projectile f_newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            f_newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();
        }
    }
}
