using UnityEngine;

public class FrogneoidStunState : EnemyState
{
    private Enemy_Frogneoid enemy;
    public FrogneoidStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, EnemyData _enemyData, string _animBoolName,Enemy_Frogneoid _frogneoid) : base(_enemyBase, _stateMachine, _enemyData, _animBoolName)
    {
        this.enemy = _frogneoid;
    }
}
