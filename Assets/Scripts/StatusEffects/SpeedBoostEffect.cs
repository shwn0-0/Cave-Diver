using System.Collections;
using UnityEngine;

class SpeedBoostEffect : IStatusEffect
{
    private readonly float _amount;
    private readonly float _duration;

    public float Duration => _duration;

    public SpeedBoostEffect(float amount, float duration)
    {
        _amount = amount;
        _duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.MoveSpeedMultiplier += _amount;
        yield return new WaitForSeconds(_duration);
        target.MoveSpeedMultiplier -= _amount;
    }
}