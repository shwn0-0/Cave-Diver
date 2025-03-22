using UnityEngine;

[CreateAssetMenu(fileName = "Player Abilities Config", menuName = "Configs/PlayerAbilitiesConfig")]
class PlayerAbilitiesConfig : ScriptableObject
{
    [Header("Boost Ability")]
    [SerializeField] private float _boostAbilityDuration = 2.0f;
    [SerializeField] private float _boostAbilityCooldown = 2.0f;
    [SerializeField] private float _boostAbilityAmount = 1.0f;
    public float BoostAbilityAmount => _boostAbilityAmount;
    public float BoostAbilityCooldown => _boostAbilityCooldown;
    public float BoostAbilityDuration => _boostAbilityDuration;

    [Header("Sheild Ability")]
    [SerializeField] private float _shieldAbilityDuration = 2.0f;
    [SerializeField] private float _shieldAbilityCooldown = 2.0f;
    [SerializeField] private float _shieldAbilityHealPercentage = 0.1f;
    public float ShieldAbilityCooldown => _shieldAbilityCooldown;
    public float ShieldAbilityDuration => _shieldAbilityDuration;
    public float ShieldAbilityHealPercentage => _shieldAbilityHealPercentage;

    [Header("C4 Ability")]
    [SerializeField] private float _c4AbilityCooldown = 2.0f;
    [SerializeField] private float _c4AbilityDamage = 10.0f;
    [SerializeField] private float _c4AbilityDuration = 2.0f;
    [SerializeField] private float _c4AbilityStunDuration = 2.0f;
    [SerializeField] private float _c4AbilityKnockbackForce = 1.0f;
    public float C4AbilityCooldown => _c4AbilityCooldown;
    public float C4AbilityDamage => _c4AbilityDamage;
    public float C4AbilityDuration => _c4AbilityDuration;
    public float C4AbilityKnockbackForce => _c4AbilityKnockbackForce;
    public float C4AbilityStunDuration => _c4AbilityStunDuration;

    [Header("Lure Ability")]
    [SerializeField] private float _lureAbilityCooldown = 2.0f;
    [SerializeField] private float _lureAbilityDuration = 2.0f;
    public float LureAbilityCooldown => _lureAbilityCooldown;
    public float LureAbilityDuration => _lureAbilityDuration;
}