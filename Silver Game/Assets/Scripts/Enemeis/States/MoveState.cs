using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{

    protected D_MoveState stateData; 

    protected bool isDetectingWall;
    protected bool isDetectingledge;
    protected bool isPlayerInMinAgroRange;

    public MoveState(Entity entity, FinalStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;

        isDetectingledge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();
    }

    public override void Enter()
    {
        base.Enter();
        entity.SetVelocity(stateData.movementSpeed);

        isDetectingledge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }


    public override void Exit()
    {
        base.Exit();
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        isDetectingledge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

}
