using System.Collections;
using UnityEngine;

class LuredEffect : IStatusEffect
{
    private readonly Rigidbody2D _target;

    public float Duration { get; set; }

    public LuredEffect(float duration, Rigidbody2D target)
    {
        Duration = duration;
        _target = target;
    }

    public IEnumerator Apply(Status status)
    {
        if (status is EnemyStatus enemyStatus)
        {
            Rigidbody2D oldTarget = enemyStatus.Target;
            enemyStatus.Target = _target;
            while (Duration > 0f)
            {
                yield return new WaitForEndOfFrame();
                Duration -= Time.deltaTime;
            }
            enemyStatus.Target = oldTarget;
        }
        else
        {
            Debug.LogError($"Lure applied to invalid status {status.GetType().Name}");
        }
    }
}