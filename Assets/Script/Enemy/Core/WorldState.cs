using System.Collections.Generic;

public enum WorldStateKey
{
    PlayerInRange,
    HasFood,
    IsHungry,
    HasTarget,
    CanAttack
}

public class WorldState : Dictionary<WorldStateKey, bool>
{
    public WorldState Clone()
    {
        var clone = new WorldState();
        foreach (var kvp in this) clone.Add(kvp.Key, kvp.Value);
        return clone;
    }
}
