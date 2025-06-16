
public abstract class GoapGoal
{
    public string GoalName { get; protected set; }
    public int Priority { get; protected set; } = 1;

    public abstract bool IsGoalMet(WorldState state);
    public abstract WorldState GetDesiredState();
}