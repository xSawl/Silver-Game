using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayerState : State
{
    protected bool turnImmdiately;
    protected bool isPlayerInMinAgroRange;
    protected bool isAllTurnsDone;
    protected bool isAllTurnsTimeDone;

    protected float lastTurnTime;

    protected int amountOfTurnDone;

    protected D_LookForPlayerState stateData;

    public LookForPlayerState(Entity entity, FinalStateMachine stateMachine, string animBoolName, D_LookForPlayerState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMaxAgroRange();
    }

    public override void Enter()
    {
        base.Enter();
        isAllTurnsDone = false;
        isAllTurnsTimeDone = false;

        lastTurnTime = startTime;
        amountOfTurnDone = 0;

        entity.SetVelocity(0f);
    }


    public override void Exit()
    {
        base.Exit();
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(turnImmdiately)
        {
            entity.Flip();
            lastTurnTime = Time.time;
            amountOfTurnDone++;
            turnImmdiately = false;
        }

        else if(Time.time >= lastTurnTime + stateData.timeBetweenTurns && !isAllTurnsDone) 
        {
            entity.Flip();
            lastTurnTime = Time.time;
            amountOfTurnDone++;
        }

        if(amountOfTurnDone >= stateData.amountOfTurns) 
        {
            isAllTurnsDone = true;
        }

        if (Time.time >= lastTurnTime + stateData.timeBetweenTurns && isAllTurnsDone) 
        {
            isAllTurnsTimeDone = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetTurnImmediately(bool flip)
    {
        turnImmdiately = flip;
    }
}
