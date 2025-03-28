using UnityEngine;

[CreateAssetMenu(menuName = "Configs/UpgradesConfig")]
class UpgradesConfig : ScriptableObject
{
    [SerializeField] float _bonusAttackDamage;
    [SerializeField] float _bonusAttackSpeed;
    [SerializeField] float _bonusAblityHaste;
    [SerializeField] float _bonusMoveSpeed;
    [SerializeField] float _bonusHealth;
    [SerializeField] float _bonusShield;

    public float BonusAttackDamage => _bonusAttackDamage;
    public float BonusAttackSpeed => _bonusAttackSpeed;
    public float BonusAblityHaste => _bonusAblityHaste;
    public float BonusMoveSpeed => _bonusMoveSpeed;
    public float BonusHealth => _bonusHealth;
    public float BonusShield => _bonusShield;
}