using UnityEngine;

[CreateAssetMenu(fileName = "Ability UI Config", menuName = "Configs/AbilityUIConfig")]
class AbilityUIConfig : ScriptableObject
{
    [SerializeField] Color _lockedColor;
    [SerializeField] Color _unlockedColor;
    [SerializeField] Color _cooldownColor;
    [SerializeField] Sprite _boostAbilityIcon;
    [SerializeField] Sprite _shieldAbilityIcon;
    [SerializeField] Sprite _c4AbilityIcon;
    [SerializeField] Sprite _lureAbilityIcon;


    public Color LockedColor => _lockedColor;
    public Color UnlockedColor => _unlockedColor;
    public Color CooldownColor => _cooldownColor;

    public Sprite GetSprite (IAbility ability)
    {
        if (ability is BoostAbility)
            return _boostAbilityIcon;

        if (ability is ShieldAbility)
            return _shieldAbilityIcon;

        if (ability is C4Ability)
            return _c4AbilityIcon;

        if (ability is LureAbility)
            return _lureAbilityIcon;
        
        return null;
    }
}