using System.Collections;
using UnityEngine;

class LuredEffect : IStatusEffect
{
    private readonly float _duration;
    private readonly Transform _target;

    public LuredEffect(float duration, Transform target)
    {
        _duration = duration;
        _target = target;
    }

    public IEnumerator Apply(Status status)
    {
        if (status is EnemyStatus enemyStatus)
        {
            Transform oldTarget = enemyStatus.Target;
            enemyStatus.Target = _target;
            yield return new WaitForSeconds(_duration);
            enemyStatus.Target = oldTarget;
        }
        else
        {
            Debug.LogError($"Lure applied to invalid status {status.GetType().Name}");
        }
    }
}