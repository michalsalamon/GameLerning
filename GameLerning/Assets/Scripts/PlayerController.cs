using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
[RequireComponent (typeof (Rigidbody))]

public class PlayerController : MonoBehaviour
{
    private Player playerInput;
    private Rigidbody RB;

    private void Awake()
    {
        playerInput = GetComponent<Player>();
        RB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer(playerInput.MoveVelocity);
        LookAtMouse(playerInput.Point);
    }

    private void LookAtMouse(Vector3 f_point)
    {
        f_point.y = transform.position.y;
        transform.LookAt(f_point);
    }

    private void MovePlayer (Vector3 f_velocity)
    {
        RB.MovePosition(RB.position + f_velocity * Time.deltaTime);
    }
}
