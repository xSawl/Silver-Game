using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_ChargeState : ChargeState
{
    private Enemy1 enemy;

    public E1_ChargeState(Entity entity, FinalStateMachine stateMachine, string animBoolName, D_ChargeState chargeStateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, chargeStateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isChargeTimeOver)
        {

            if(!isDetectingLedge || isDetectingWall)
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }

            else if(isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
