using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(GunController))]

public class PlayerInput : LivingEntity
{
    private Vector3 moveVector = Vector3.zero;
    private Vector3 moveVelocity;
    public Vector3 MoveVelocity
    {
        get { return moveVelocity; }
    }

    private Ray ray;
    private Vector3 point;
    public Vector3 Point
    {
        get { return point; }
    }
    [SerializeField] private float playerSpeed = 5;

    private Camera viewCamera;
    private GunController gunController;

    protected override void Start()
    {
        base.Start();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
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
        Plane f_groundPlane = new Plane(Vector3.up, Vector3.zero);
        float f_rayDistance;
        if (f_groundPlane.Raycast(ray, out f_rayDistance))
        {
            point = ray.GetPoint(f_rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
        }

        //weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
