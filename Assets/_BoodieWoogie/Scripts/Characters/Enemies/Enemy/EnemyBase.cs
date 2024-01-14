using UnityEngine;

public enum EEnemyState
{
    Idle,
    Attack
}
public abstract class EnemyBase : MonoBehaviour
{
    protected EEnemyState enemyState = EEnemyState.Idle;
    protected abstract void Attack();

}
