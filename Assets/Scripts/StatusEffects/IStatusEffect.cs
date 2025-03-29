using System.Collections;

interface IStatusEffect
{
    public float Duration { get; }
    public IEnumerator Apply(Status target);
}