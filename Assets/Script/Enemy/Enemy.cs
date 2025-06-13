using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using EnemyEnum.Enums;
public class Enemy : EnemyEntity
{
   
   [SerializeField] protected GameObject counterImage;
   [HideInInspector] public float lastTimeAttacked;

 

   public EnemyStateMachine stateMachine { get; private set; }
   public string lastAnimBoolName { get; private set; }
   public EnemyType enemyType = EnemyType.None; // Add this line
   private List<Enemy> allEnemies;
   private List<Enemy> frogNeoids;
   private List<Enemy> angels;
   protected override void Awake()
   {
      base.Awake();
      stateMachine = new EnemyStateMachine();
      enemyData.defaultMoveSpeed = enemyData.movementSpeed;
   }
   protected override void Start()
   {
      base.Start();
      // 初始化状态机
      frogNeoids = EnemyManager.instance.GetEnemies(EnemyType.Frogneoid);

      if (frogNeoids.Count > 0)
      {
         Debug.Log($"Found {frogNeoids.Count} Frogneoid(s) in the scene.");
         foreach (Enemy frog in frogNeoids)
         {
            // You can now do something with each frogneoid enemy
            Debug.Log("Frogneoid name: " + frog.gameObject.name);
            // frog.SomeMethod(); // If Enemy class has other public methods
         }
      }
      else
      {
         Debug.Log("No Frogneoids found.");
      }

      // Example: Get all Angel enemies
      angels = EnemyManager.instance.GetEnemies(EnemyType.Angel);
      if (angels.Count > 0)
      {
         Debug.Log($"Found {angels.Count} Angel(s).");
         // ... do something with the angels
      }
      // Get all active enemies at once
       allEnemies = EnemyManager.instance.GetAllActiveEnemies();

      if (allEnemies.Count > 0)
      {
         Debug.Log($"Found a total of {allEnemies.Count} active enemies.");
         foreach (Enemy enemy in allEnemies)
         {
            // Now you can do something with each enemy
            Debug.Log($"Enemy name: {enemy.gameObject.name}, Type: {enemy.enemyType}");

            // Example: Make all enemies perform an action if they can
            // if (enemy is Enemy_Frogneoid frog) // Example of type checking if needed
            // {
            //    frog.SomeFrogneoidSpecificAction();
            // }
            // else if (enemy is Enemy_Skeleton skeleton)
            // {
            //    skeleton.SomeSkeletonSpecificAction();
            // }
            // else
            // {
            //    enemy.GenericEnemyAction(); // Assuming Enemy base class has such a method
            // }
         }
      }
      else
      {
         Debug.Log("No active enemies found in the scene.");
      }
   
   }
   
   protected override void Update()
   {
      base.Update();
      // 检查 stateMachine 和 currentState 是否为 null
      if (stateMachine != null && stateMachine.currentState != null)
      {
         stateMachine.currentState.Update();
      }
      else
      {
         Debug.LogWarning("State machine or current state is not initialized.");
      }
      var playerHit = IsPlayerDetected();

      // 检查 RaycastHit2D 的 collider 是否为 null
      if (playerHit.collider != null)
      {
         
      }
      else
      {
         
      }
      
   }

   public virtual void AssignLastAnimName(string _animBoolName)
   {
      lastAnimBoolName = _animBoolName;
   }
   public virtual RaycastHit2D IsPlayerDetected()=>Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, enemyData.playerCheckDistance, enemyData.whatIsPlayer);
   
   
   public virtual void FreezeTime(bool _timeFreeze)
   {
      if (_timeFreeze)
      {
         enemyData.movementSpeed = 0;
         anim.speed = 0;
      }
      else
      {
         enemyData.movementSpeed = enemyData.defaultMoveSpeed;
         anim.speed = 1;
      }
      
   }
   public virtual void FreezeTimeFor(float _duration)=>StartCoroutine(FreezeTimerCoroutine(_duration));
   protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
   {
      FreezeTime(true);
      yield return new WaitForSeconds(_seconds);
      FreezeTime(false);
   }

   public void ApplyKnockback(Vector2 force)
   {
      Debug.LogError("apply knockback");
      if (this.facingDirection == PlayerManager.instance.player.facingDirection)
      {
         rb.linearVelocity = new Vector2(force.x * facingDirection, force.y);
      }
      rb.linearVelocity = new Vector2(force.x * - facingDirection, force.y );
   }

   public virtual void OpenCounterAttackWindow()
   {
      enemyData.canBeStunned = true;
      counterImage.SetActive(true);
   }

   public virtual void CloseCounterAttackWindow()
   {
      enemyData.canBeStunned = false;
      counterImage.SetActive(false);
   }

   
   public virtual bool CanBeStunned()
   {
      if (enemyData.canBeStunned)
      {
         CloseCounterAttackWindow();
         return true;
      }
      return false;
   }
   public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();
}
