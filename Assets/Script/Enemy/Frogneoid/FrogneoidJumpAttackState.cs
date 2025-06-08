using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FrogneoidJumpAttackState : EnemyState
{
    private Enemy_Frogneoid frogneoid;
    private Vector2 attackDirection;

  
    public FrogneoidJumpAttackState
        (Enemy _enemyBase, EnemyStateMachine _stateMachine,
            EnemyData _enemyData,string _animBoolName,Enemy_Frogneoid _frogneoid) 
        : base(_enemyBase, _stateMachine, _enemyData,_animBoolName)
    {
        this.frogneoid = _frogneoid;
    }

    public override void Enter()
    {
        base.Enter();
        
        // 计算跳跃方向
        attackDirection = (PlayerManager.instance.player.transform.position - frogneoid.transform.position).normalized;
        frogneoid.rb.linearVelocity = new Vector2(attackDirection.x * frogneoid.jumpAttackForce, 
            frogneoid.jumpAttackForce);
    }

    public override void Update()
    {
        base.Update();

        // 落地检测
        if(frogneoid.IsGroundDetected())
        {
            // 触发冲击波
            frogneoid.CreateGroundImpactEffect();
            stateMachine.ChangeState(frogneoid.idleState);
        }
    }
}