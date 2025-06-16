using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using EnemyEnum.Enums;
using System.Collections.Generic;
using UnityEngine;

// ========================
// 指挥官系统
// ========================
public class CommanderAI : MonoBehaviour
{
    // 指挥官状态
    public enum CommanderState
    {
        Planning,
        Executing,
        Idle
    }

    public CommanderState currentState = CommanderState.Idle;
    private GoapPlanner commanderPlanner = new GoapPlanner();
    private List<CommanderAction> availableCommanderActions = new List<CommanderAction>();
    private Queue<CommanderAction> currentCommanderPlan;

    void Start()
    {
        InitializeCommanderActions();
    }

    void Update()
    {
        switch (currentState)
        {
            case CommanderState.Planning:
                PlanSquadActions();
                break;

            case CommanderState.Executing:
                ExecuteCommanderPlan();
                break;
        }
    }

    // 初始化指挥官动作
    private void InitializeCommanderActions()
    {
        availableCommanderActions.Add(new OrderAttackPlayer());
        availableCommanderActions.Add(new OrderSecureArea());
        availableCommanderActions.Add(new OrderFindResources());
    }

    // 规划小队行动
    private void PlanSquadActions()
    {
        WorldState squadWorldState = GetSquadWorldState();
        CommanderGoal currentGoal = GetCurrentCommanderGoal();

        currentCommanderPlan = commanderPlanner.Plan(gameObject, availableCommanderActions, squadWorldState, currentGoal);

        if (currentCommanderPlan != null && currentCommanderPlan.Count > 0)
        {
            currentState = CommanderState.Executing;
        }
    }

    // 执行指挥官计划
    private void ExecuteCommanderPlan()
    {
        if (currentCommanderPlan.Count > 0)
        {
            CommanderAction currentAction = currentCommanderPlan.Peek();

            if (currentAction.Execute(this))
            {
                currentCommanderPlan.Dequeue();

                if (currentCommanderPlan.Count == 0)
                {
                    currentState = CommanderState.Idle;
                }
            }
        }
    }

    // 获取小队世界状态
    private WorldState GetSquadWorldState()
    {
        WorldState state = new WorldState();

        // 从所有敌人收集状态信息
        List<Enemy> allEnemies = EnemyManager.instance.GetAllActiveEnemies();
        foreach (Enemy enemy in allEnemies)
        {
            // 收集敌人状态信息
            // 例如：state[WorldStateKey.HasAmmo] |= enemy.HasAmmo;
        }

        return state;
    }

    // 命令特定类型的敌人
    public void OrderEnemyType(EnemyType type, GoapGoal goal)
    {
        List<Enemy> enemies = EnemyManager.instance.GetEnemies(type);

        foreach (Enemy enemy in enemies)
        {
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.AssignCommanderGoal(goal);
            }
        }
    }
}

// 指挥官动作基类
public abstract class CommanderAction : GoapAction
{
    // 添加指挥官特有的方法
    public abstract bool Execute(CommanderAI commander);
}

// 具体指挥官动作：命令攻击玩家
public class OrderAttackPlayer : CommanderAction
{
    public OrderAttackPlayer()
    {
        ActionName = "Order Attack Player";
        Preconditions[WorldStateKey.PlayerVisible] = true;
        Effects[WorldStateKey.PlayerEngaged] = true;
    }

    public override bool Execute(CommanderAI commander)
    {
        // 命令所有天使攻击玩家
        commander.OrderEnemyType(EnemyType.Angel, new AttackPlayerGoal());

        // 命令蛙人提供支援
        commander.OrderEnemyType(EnemyType.Frogneoid, new SupportAttackGoal());

        return true;
    }
}

// 具体指挥官动作：命令寻找资源
public class OrderFindResources : CommanderAction
{
    public OrderFindResources()
    {
        ActionName = "Order Find Resources";
        Preconditions[WorldStateKey.HasResources] = false;
        Effects[WorldStateKey.HasResources] = true;
    }

    public override bool Execute(CommanderAI commander)
    {
        // 命令蛙人寻找资源
        commander.OrderEnemyType(EnemyType.Frogneoid, new FindResourcesGoal());
        return true;
    }
}

