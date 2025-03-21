using UnityEngine;

[CreateAssetMenu(fileName = "Ability UI Config", menuName = "Configs/AbilityUIConfig")]
class AbilityUIConfig : ScriptableObject
{
    [SerializeField] Color _lockedColor;
    [SerializeField] Color _unlockedColor;
    [SerializeField] Color _cooldownColor;


    public Color LockedColor => _lockedColor;
    public Color UnlockedColor => _unlockedColor;
    public Color CooldownColor => _cooldownColor;

    // FIXME: Implement this
    public Sprite GetSprite (IAbility ability) => null;
}