using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single};
    [SerializeField] private FireMode fireMode;

    [SerializeField] private Transform[] projectileSpawner;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float msBetweenShot = 100;
    [SerializeField] private float muzzleVelocity = 35;
    [SerializeField] private int burstCount;


    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjector;
    private MuzzleFlash muzzleFlash;

    private float nextShotTime;

    private bool triggerReleasedSinceLastShot;
    private int shotsRemainingInBurst; 

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
    }

    private void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawner.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShot / 1000;
                Projectile f_newProjectile = Instantiate(projectile, projectileSpawner[i].position, projectileSpawner[i].rotation);
                f_newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
