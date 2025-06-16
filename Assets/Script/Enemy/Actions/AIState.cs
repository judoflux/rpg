public abstract class AIState : EnemyState
{
    protected EnemyAI context;
    protected AIState(Enemy _enemyBase, EnemyStateMachine _stateMachine, EnemyData _enemyData, string _animBoolName) : base(_enemyBase, _stateMachine, _enemyData, _animBoolName)
    {
    }

    // 新增GOAP相关方法
    public virtual void OnActionStart() { }
    public virtual void OnActionComplete() { }
    public virtual void OnActionFailed() { }
    
    

    public void SetContext(EnemyAI context)
    {
        this.context = context;
    }
}