using UnityEngine;

public class FrogneoidIdleState : FrogneoidGroundedState
{
    public FrogneoidIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, EnemyData _enemyData, string _animBoolName,Enemy_Frogneoid _frogneoid) : base(_enemyBase, _stateMachine, _enemyData, _animBoolName,_frogneoid)
    {
        this.enemy = _frogneoid;
    }
    public override void Enter()
    {
        base.Enter();
        stateTimer = enemyData.idleTime;
    }
    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
