using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerInput))]
[RequireComponent (typeof (Rigidbody))]

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody RB;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
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
