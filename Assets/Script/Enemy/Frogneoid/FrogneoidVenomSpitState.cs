using UnityEngine;

public class FrogneoidVenomSpitState : EnemyState
{
    private Enemy_Frogneoid enemy;
    public FrogneoidVenomSpitState(Enemy _enemyBase, EnemyStateMachine _stateMachine, EnemyData _enemyData, string _animBoolName,Enemy_Frogneoid _frogneoid) : base(_enemyBase, _stateMachine, _enemyData, _animBoolName)
    {
        this.enemy = _frogneoid;
    }
}
