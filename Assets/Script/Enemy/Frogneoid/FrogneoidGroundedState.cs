using UnityEngine;

public class FrogneoidGroundedState : EnemyState
{
    protected Enemy_Frogneoid enemy;
    protected Transform player;

    public FrogneoidGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, EnemyData _enemyData, string _animBoolName, Enemy_Frogneoid _frogneoid) : base(_enemyBase, _stateMachine, _enemyData, _animBoolName)
    {
        this.enemy = _frogneoid;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < 2)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
    
}
