using UnityEngine;
using EnemyEnum.Enums;
[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/EnemyData/Base Data")]
public class EnemyData : ScriptableObject
{
    EnemyType enemyType;
    [Header("Movement")] 
    public float movementSpeed = 2f;
    public float horizontalSpeed = 1f;
    public float straightJumpForce = 5f;
    public float jumpForce = 6f;
    public float grenadeReturnImpact;
    public float defaultMoveSpeed;
    public float defaultJumpForce;
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    [Header("attack info")] 
    public float attackDistance;

   
    public float attackCooldown;
    
    [Header("stun info")] 
    public float stunDuration;
    public Vector2 stunDirection;
    public bool canBeStunned;
    [Header("collision info")] 
    public LayerMask groundAndEdgeLayer;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsWall;
    public LayerMask whatIsCeiling;
    public LayerMask whatIsLedge;
    public LayerMask whatIsGround;
    public float attackCheckRadius;
    public float groundCheckDistance;
    public float wallCheckDistance;
    public float wallBackCheckDistance;
    public float ledgeCheckDistance;
    public float playerCheckDistance;
    [Header("enemy attack info")] 
    public Vector2 enemyKnockBackForce;

    public Vector2 enemySpecialKnockBackForce;
    public Vector2 bossKnockBackForce;
    [Header("slope info")]
    public float slopeCheckDistance = 0.2f;

}
