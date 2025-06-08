using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Enemy_Frogneoid : Enemy
{
    // 状态声明
    public FrogneoidIdleState idleState { get; private set; }
    public FrogneoidMoveState moveState { get; private set; }
    public FrogneoidJumpAttackState jumpAttackState { get; private set; }
    public FrogneoidVenomSpitState venomSpitState { get; private set; }
    public FrogneoidStunState stunState { get; private set; }
    public FrogneoidDeadState deadState { get; private set; }
    
    public FrogneoidBattleState battleState { get; private set; }

    [Header("Frogneoid Specific")]
    public Transform venomSpawnPoint;
    public GameObject venomProjectilePrefab;
    public float jumpAttackForce = 15f;

    protected override void Awake()
    {
        base.Awake();
        
        // 初始化状态
        idleState = new FrogneoidIdleState(this, stateMachine, enemyData, "Idle",this);
        moveState = new FrogneoidMoveState(this, stateMachine, enemyData, "Move",this);
        jumpAttackState = new FrogneoidJumpAttackState(this, stateMachine, enemyData, "JumpAttack",this);
        venomSpitState = new FrogneoidVenomSpitState(this, stateMachine, enemyData, "VenomSpit",this);
        stunState = new FrogneoidStunState(this, stateMachine, enemyData, "Stun",this);
        deadState = new FrogneoidDeadState(this, stateMachine, enemyData, "Dead",this);
        battleState = new FrogneoidBattleState(this, stateMachine, enemyData, "Battle", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    // 毒液生成方法（由动画事件调用）
    public void SpitVenomProjectile()
    {
        Instantiate(venomProjectilePrefab, venomSpawnPoint.position, Quaternion.identity);
    }

    public void CreateGroundImpactEffect()
    {
        Debug.LogWarning("Ground impact effect not implemented for Frogneoid.");
    }
}
