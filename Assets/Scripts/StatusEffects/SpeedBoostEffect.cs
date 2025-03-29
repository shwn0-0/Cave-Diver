using System.Collections;
using UnityEngine;

class SpeedBoostEffect : IStatusEffect
{
    private readonly float _amount;

    public float Duration { get; set; }

    public SpeedBoostEffect(float amount, float duration)
    {
        _amount = amount;
        Duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.MoveSpeedMultiplier += _amount;
        while (Duration > 0f)
        {
            yield return new WaitForEndOfFrame();
            Duration -= Time.deltaTime;
        }
        target.MoveSpeedMultiplier -= _amount;
    }
}