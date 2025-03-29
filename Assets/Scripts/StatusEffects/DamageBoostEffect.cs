using System.Collections;
using UnityEngine;

class DamageBoostEffect : IStatusEffect
{
    private readonly float _amount;

    public float Duration { get; set; }

    public DamageBoostEffect(float amount, float duration)
    {
        _amount = amount;
        Duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.DamageMultiplier += _amount;
        while (Duration > 0f)
        {
            yield return new WaitForEndOfFrame();
            Duration -= Time.deltaTime;
        }
        target.DamageMultiplier -= _amount;
    }
}