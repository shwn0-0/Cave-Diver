using UnityEngine;

interface IController
{
    public void ApplyKnockbackFrom(Vector2 position, float knockback);
}