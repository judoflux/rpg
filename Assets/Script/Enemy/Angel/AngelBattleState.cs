using UnityEngine;

public class AngelBattleState : EnemyState
{
    private Enemy_Angel enemy;
    private Transform player;
    private int moveDirection;
    public AngelBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine,EnemyData _enemyData, string _animBoolName,Enemy_Angel _enemy) : base(_enemyBase, _stateMachine,_enemyData, _animBoolName)
    {
        this.enemy = _enemy;
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
        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemyData.battleTime;
            if (enemy.IsPlayerDetected().distance < enemyData.attackDistance)
            {
                if (CanAttack())
                {
                    
                stateMachine.ChangeState(enemy.attackState);
                }
            }
            else
            {
                if (stateTimer < 0 || Vector2.Distance(player.transform.position,enemy.transform.position)>10)
                {
                    stateMachine.ChangeState(enemy.idleState);
                }
            }
        }
        if (player.position.x > enemy.transform.position.x)
        {
            moveDirection = 1;
        }else if (player.position.x < enemy.transform.position.x)
        {
            moveDirection = -1;
        }
        enemy.SetVelocityX(enemyData.movementSpeed);
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemyData.attackCooldown)
        {
            enemy.lastTimeAttacked = Time.time;//This means last time attacked turns to time.time current time , we will 
            //need to check if attack was hitten later
            return true;
        }
        
        return false; 
    }
}
