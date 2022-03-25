using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform weaponHold;
    [SerializeField] private Gun[] guns;

    private Gun equippedGun;

    public float GunHeight
    { get { return weaponHold.position.y; } }

    private void Start()
    {
        if (guns.Length > 0)
        {
            EquipGun(1);
        }
    }

    public void EquipGun(int number)
    {
        if (guns.Length >= number)
        {
            if (equippedGun != null)
            {
                Destroy(equippedGun.gameObject);
            }
            equippedGun = Instantiate(guns[number - 1], weaponHold.position, weaponHold.rotation);
            equippedGun.transform.parent = weaponHold;
        }
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public void Aim (Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }
}
