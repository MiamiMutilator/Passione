using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : IAction
{
    private readonly PlayerController player;
    private readonly float dashVelocity;
    private Vector3 lastMoveDirection;
    private readonly CharacterController controller;

    public Dash(PlayerController player, CharacterController controller, float dashVelocity)
    {
        this.player = player;
        this.controller = controller;
        this.dashVelocity = dashVelocity;

        lastMoveDirection = Vector3.forward;
    }

    public void OnActivation()
    {
        lastMoveDirection = player.lastDirection;
        Vector3 targetMovement = lastMoveDirection * dashVelocity;

        controller.Move(targetMovement * Time.deltaTime);
    }
}
