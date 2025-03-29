using System.Collections;
using UnityEngine;

class ShieldEffect : IStatusEffect
{
    public float Duration { get; set; }


    public ShieldEffect(float duration)
    {
        Duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.IsInvulnerable = true;
        while (Duration > 0f)
        {
            yield return new WaitForEndOfFrame();
            Duration -= Time.deltaTime;
        }
        target.IsInvulnerable = false;
    }
}