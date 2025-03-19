using System.Collections;
using UnityEngine;

class DamageBoostEffect : IStatusEffect
{
    private readonly float _amount;
    private readonly float _duration;

    public DamageBoostEffect(float amount, float duration)
    {
        _amount = amount;
        _duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.DamageMultiplier += _amount;
        yield return new WaitForSeconds(_duration);
        target.DamageMultiplier -= _amount;
    }
}