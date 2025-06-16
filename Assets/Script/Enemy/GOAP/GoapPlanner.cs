using UnityEngine;

public class GoapPlanner
{
    // ... (GOAP????)
}

public abstract class GoapAction
{
    // ??????????
    public System.Type LinkedState { get; protected set; }

    public virtual bool Execute(GameObject agent, EnemyAI aiController)
    {
        // ??????
        aiController.SwitchState(LinkedState);
        return true;
    }
}
