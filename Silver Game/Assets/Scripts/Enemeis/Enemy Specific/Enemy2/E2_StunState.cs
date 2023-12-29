using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class E2_StunState : StunState
{
private Enemy2 enemy;
    public E2_StunState(Entity entity, FinalStateMachine stateMachine, string animBoolName, D_StunState stateData, Enemy2 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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

        if(isStunTimeOver)
        {  
            if(performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }

            else if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }

            else 
            {
                enemy.lookForPlayerState.SetTurnImmediately(true);
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
        

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }
}
