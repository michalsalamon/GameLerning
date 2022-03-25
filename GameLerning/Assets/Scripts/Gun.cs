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
    [SerializeField] private int projectilesPerMag = 30;
    [SerializeField] private float reloadTime = 1.5f;

    [Header ("Recoil")]
    [SerializeField] private Vector2 recoilMinMax = new Vector2(0.15f, 0.25f);
    [SerializeField] private float smoothMoveTime = 0.1f;
    [SerializeField] private float recoilAngle = 0;
    [SerializeField] private float maxRecoilAngle = 30;
    [SerializeField] private Vector2 recoilAngleMinMax = new Vector2(10, 25);
    [SerializeField] private float smoothRecoilTime = 0.1f;
    

    [Header ("Effects")]
    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjector;
    private MuzzleFlash muzzleFlash;

    private float nextShotTime;

    private bool triggerReleasedSinceLastShot = true;
    private int shotsRemainingInBurst;
    private int projectilesRemainingInMag;
    private bool isReloading;

    private float recoilRotSmoothDampVelocity;
    private Vector3 recoilSmoothDampVelocity;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    private void LateUpdate()
    {
        //animate recoli
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, smoothMoveTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, smoothRecoilTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
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
            projectilesRemainingInMag--;
            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(recoilMinMax.x, recoilMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, maxRecoilAngle);
        }
    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 60;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float f_reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * f_reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
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
