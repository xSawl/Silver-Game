using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : State
{
    protected D_PlayerDetectted stateData;

    protected bool isPlayerInMinAgroRange;
    protected bool isPlayerInMaxAgroRange;
    protected bool performLongRangeAction;

    public PlayerDetectedState(Entity entity, FinalStateMachine stateMachine, string animBoolName, D_PlayerDetectted stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMaxAgroRange();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();
    }

    public override void Enter()
    {
        base.Enter();
        performLongRangeAction = false;
        entity.SetVelocity(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(Time.time >= startTime + stateData.longRangeActionTime)
        {
            performLongRangeAction = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
}
