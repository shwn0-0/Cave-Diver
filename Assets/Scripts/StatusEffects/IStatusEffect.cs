using System.Collections;

interface IStatusEffect
{
    public IEnumerator Apply(Status target);
}