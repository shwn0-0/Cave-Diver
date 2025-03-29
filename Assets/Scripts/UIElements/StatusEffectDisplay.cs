using System.Collections.ObjectModel;
using Unity.Mathematics;
using UnityEngine;

class StatusEffectsDisplay : MonoBehaviour
{
    [SerializeField] private Sprite _damageBoostSprite;
    [SerializeField] private Sprite _luredSprite;
    [SerializeField] private Sprite _shieldSprite;
    [SerializeField] private Sprite _speedBoostSprite;
    [SerializeField] private Sprite _stunnedSprite;

    Status _status;
    StatusEffectController[] _statusEffectDisplays;

    void Awake()
    {
        _status = GetComponentInParent<Status>();
        _statusEffectDisplays = GetComponentsInChildren<StatusEffectController>(includeInactive: true);
    }

    public void UpdateWithLatest()
    {
        ReadOnlyCollection<IStatusEffect> effects = _status.StatusEffects;
        int i;

        for (i = 0; i < math.min(_statusEffectDisplays.Length, effects.Count); i++)
        {
            _statusEffectDisplays[i].SetStatusEffect(GetSprite(effects[i]));
        }

        for  (; i < _statusEffectDisplays.Length; i++)
        {
            _statusEffectDisplays[i].Disable();
        }
    }

    private Sprite GetSprite(IStatusEffect statusEffect) => statusEffect switch
    {
        DamageBoostEffect => _damageBoostSprite,
        SpeedBoostEffect => _speedBoostSprite,
        LuredEffect => _luredSprite,
        ShieldEffect => _shieldSprite,
        StunnedEffect => _stunnedSprite,
        _ => null,
    };
}