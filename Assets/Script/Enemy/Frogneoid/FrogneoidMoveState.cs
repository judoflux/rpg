using UnityEngine;

public class FrogneoidMoveState : FrogneoidGroundedState
{
    

    public FrogneoidMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, EnemyData _enemyData, string _animBoolName, Enemy_Frogneoid _frogneoid) 
        : base(_enemyBase, _stateMachine, _enemyData, _animBoolName, _frogneoid)
    {
        this.enemy = _frogneoid;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocityX(enemyData.moveSpeed);
    }

    public override void Update()
    {
        base.Update();
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.EnemyZeroVelocity();
    }
}


