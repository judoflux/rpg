using CrashKonijn.Goap.Core;
using System.Collections.Generic;
using UnityEngine;

// ========================
// GOAP与状态机集成框架
// ========================


public class EnemyAI : Enemy
{
    // GOAP组件
    private GoapPlanner planner;
    private WorldState worldState;
    private List<GoapAction> availableActions = new List<GoapAction>();
    private List<GoapGoal> availableGoals = new List<GoapGoal>();
    private Queue<GoapAction> currentPlan;

    // 指挥官相关
    private GoapGoal commanderGoal;
    private bool hasCommanderOrder;

    // 状态机组件（使用您现有的状态机）
    public EnemyStateMachine stateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
    }

    protected override void Start()
    {
        base.Start();
        InitializeGOAP();
        InitializeStateMachine();
    }

    protected override void Update()
    {
        base.Update();

        // GOAP规划
        UpdateGOAPPlanning();

        // 状态机更新
        if (stateMachine != null && stateMachine.currentState != null)
        {
            stateMachine.currentState.Update();
        }
    }

    // 初始化GOAP
    private void InitializeGOAP()
    {
        worldState = new WorldState
        {
            { WorldStateKey.IsHungry, true },
            { WorldStateKey.HasFood, false },
            { WorldStateKey.PlayerInRange, false }
        };

        // 添加动作 - 根据敌人类型定制
        availableActions.Add(new ChasePlayerAction());
        availableActions.Add(new AttackPlayerAction());

        // 添加目标
        availableGoals.Add(new AttackPlayerGoal(10));
        availableGoals.Add(new NotHungryGoal(5));
    }

    // 初始化状态机
    private void InitializeStateMachine()
    {
        // 创建具体状态（根据您的实现）
        var wanderState = new WanderState(enemyBase, stateMachine, enemyData, "wander");
        var chaseState = new ChaseState(enemyBase, stateMachine, enemyData, "chase");
        var attackState = new AttackState(enemyBase, stateMachine, enemyData, "attack");

        // 设置初始状态
        stateMachine.Initialize(wanderState);
    }

    // 接收指挥官命令
    public void AssignCommanderGoal(GoapGoal goal)
    {
        commanderGoal = goal;
        hasCommanderOrder = true;
        currentPlan = null; // 强制重新规划
    }

    // 清除指挥官命令
    public void ClearCommanderGoal()
    {
        commanderGoal = null;
        hasCommanderOrder = false;
        currentPlan = null;
    }

    // GOAP规划更新
    private void UpdateGOAPPlanning()
    {
        if (currentPlan == null || currentPlan.Count == 0)
        {
            GoapGoal goalToAchieve = hasCommanderOrder ? commanderGoal : GetHighestPriorityGoal();

            if (goalToAchieve != null)
            {
                currentPlan = planner.Plan(gameObject, availableActions, worldState, goalToAchieve);

                if (currentPlan != null && currentPlan.Count > 0)
                {
                    ExecuteNextAction();
                }
            }
        }
    }

    // 执行下一个动作
    private void ExecuteNextAction()
    {
        GoapAction nextAction = currentPlan.Dequeue();

        // 查找动作对应的状态
        System.Type stateType = nextAction.LinkedState;
        if (stateType != null)
        {
            // 切换到动作对应的状态
            EnemyState nextState = GetStateByType(stateType);
            if (nextState != null)
            {
                stateMachine.ChangeState(nextState);
            }
        }
        else
        {
            // 直接执行没有关联状态的动作
            nextAction.Execute(gameObject, this);
        }
    }

    // 根据类型获取状态
    private EnemyState GetStateByType(System.Type stateType)
    {
        // 实际实现中需要维护状态实例的映射
        // 这里简化处理
        if (stateType == typeof(ChaseState)) return new ChaseState(...);
        if (stateType == typeof(AttackState)) return new AttackState(...);
        return null;
    }
}