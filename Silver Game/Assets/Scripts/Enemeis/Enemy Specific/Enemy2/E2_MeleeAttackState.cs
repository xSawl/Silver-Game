using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_MeleeAttackState : MeleeAtackState
{
    private Enemy2 enemy;

    public E2_MeleeAttackState(Entity entity, FinalStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData, Enemy2 enemy) : base(entity, stateMachine, animBoolName, attackPosition, stateData)
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

        if(isAnimationFinished)
        {
            if(isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }

            else
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }


    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }

}
