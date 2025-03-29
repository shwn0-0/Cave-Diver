using System.Collections;

class DemoEffect : IStatusEffect
{
    public float Duration => float.PositiveInfinity;

    public IEnumerator Apply(Status target)
    {
        target.DamageMultiplier = 0f;
        target.Health = 1f;
        yield return null;
    }
}