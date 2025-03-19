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
        Transform oldTarget = status.Target;
        status.Target = _target;
        yield return new WaitForSeconds(_duration);
        status.Target = oldTarget;
    }
}