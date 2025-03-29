using System.Collections;
using UnityEngine;

class StunnedEffect : IStatusEffect
{
    public float Duration { get; set; }

    public StunnedEffect(float duration)
    {
        Duration = duration;
    }

    public IEnumerator Apply(Status target)
    {
        target.IsStunned = true;
        while (Duration > 0f)
        {
            yield return new WaitForEndOfFrame();
            Duration -= Time.deltaTime;
        }
        target.IsStunned = false;
    }
}