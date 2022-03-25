using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(GunController))]

public class Player : LivingEntity
{
    [SerializeField] private Crosshairs crosshairs;

    private Vector3 moveVector = Vector3.zero;
    private Vector3 moveVelocity;
    public Vector3 MoveVelocity
    { get { return moveVelocity; }}

    private Ray ray;
    private Vector3 point;
    public Vector3 Point
    {
        get { return point; }
    }
    [SerializeField] private float playerSpeed = 5;

    private Camera viewCamera;
    private GunController gunController;

    private void Awake()
    {
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;

        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnNewWave(int waveNumber)
    {
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //move input
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.z = Input.GetAxisRaw("Vertical");
        moveVelocity = moveVector.normalized * playerSpeed;

        //look input
        ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane f_groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float f_rayDistance;

        if (f_groundPlane.Raycast(ray, out f_rayDistance))
        {
            point = ray.GetPoint(f_rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            crosshairs.transform.position = point;
            crosshairs.DetectTarget(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }

        //weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        //gunselection
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunController.EquipGun(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gunController.EquipGun(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gunController.EquipGun(3);
        }
    }
}
