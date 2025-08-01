using UnityEngine;

public class AngelMoveState : AngelGroundedState
{
   public AngelMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine,EnemyData _enemyData, string _animBoolName, Enemy_Angel _enemy) : base(_enemyBase, _stateMachine,_enemyData, _animBoolName, _enemy)
   {
      this.enemy = _enemy;
   }


   public override void Enter()
   {
      base.Enter();
   }

   public override void Exit()
   {
      base.Exit();
   }
   public override void Update()
   {
      base.Update();
      enemy.SetVelocityX(enemyData.movementSpeed * enemy.facingDirection);
      if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
      {
         enemy.Flip();
         stateMachine.ChangeState(enemy.moveState);
      }
   }
}
