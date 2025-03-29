using System.Collections;

interface IStatusEffect
{
    public float Duration { get; set; }
    public IEnumerator Apply(Status target);
}