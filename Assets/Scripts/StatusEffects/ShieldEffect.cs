using System.Collections;
using UnityEngine;

class ShieldEffect : IStatusEffect
{
    private readonly float _duration;

    public ShieldEffect(float duration)
    {
        _duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.IsInvulnerable = true;
        yield return new WaitForSeconds(_duration);
        target.IsInvulnerable = false;
    }
}