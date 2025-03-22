using UnityEngine;

[CreateAssetMenu(menuName = "Configs/StatusConfig")]
class StatusConfig : ScriptableObject
{
    [Header("Attacking")]
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackAngle;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackKnockback;
    [SerializeField] private float _attackSelfKnockforward;
    public float AttackDamage => _attackDamage;
    public float AttackAngle => _attackAngle;
    public float AttackRange => _attackRange;
    public float AttackSpeed => _attackSpeed;
    public float AttackKnockback => _attackKnockback;
    public float AttackSelfKnockforward => _attackSelfKnockforward;
    
    [Header("Health")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _maxShields;
    public float MaxHealth => _maxHealth;
    public float MaxShield => _maxShields;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _knockbackFriction;
    public float MoveSpeed => _moveSpeed;
    public float KnockbackFriction => _knockbackFriction;
}