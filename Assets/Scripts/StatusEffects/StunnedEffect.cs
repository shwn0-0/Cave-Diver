using System.Collections;
using UnityEngine;

class StunnedEffect : IStatusEffect
{
    private readonly float _duration;

    public StunnedEffect(float duration)
    {
        _duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.IsStunned = true;
        yield return new WaitForSeconds(_duration);
        target.IsStunned = false;
    }
}