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
    [SerializeField] Sprite _noAbilityIcon;


    public Color LockedColor => _lockedColor;
    public Color UnlockedColor => _unlockedColor;
    public Color CooldownColor => _cooldownColor;

    public Sprite GetSprite (IAbility ability) => ability switch
    {
        BoostAbility => _boostAbilityIcon,
        ShieldAbility => _shieldAbilityIcon,
        C4Ability => _c4AbilityIcon,
        LureAbility => _lureAbilityIcon,
        _ => _noAbilityIcon
    };
}