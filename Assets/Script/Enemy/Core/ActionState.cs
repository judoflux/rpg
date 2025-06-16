using System;
using UnityEngine;

public abstract class GoapAction
{
    public string ActionName { get; protected set; }
    public float Cost { get; protected set; } = 1.0f;
    public WorldState Preconditions { get; } = new WorldState();
    public WorldState Effects { get; } = new WorldState();
    public System.Type LinkedState { get; protected set; }

    public virtual bool IsValid(WorldState state)
    {
        foreach (var precondition in Preconditions)
        {
            // 1. 检查前提条件是否存在于世界状态中
            if (!state.ContainsKey(precondition.Key))
                return false;

            // 2. 检查前提条件的值是否匹配
            if (state[precondition.Key] != precondition.Value)
                return false;
        }
        return true;
    }

    public virtual bool Execute(GameObject agent, EnemyAI aiController)
    {
        if (LinkedState != null)
        {
            aiController.SwitchState(LinkedState);
            return true;
        }
        return false;
    }
}