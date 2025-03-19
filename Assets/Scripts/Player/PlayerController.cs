using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;
    [SerializeField] private GameObject _c4Prefab;
    [SerializeField] private GameObject _lurePrefab;

    private Vector2 _walkingDisplacement = Vector2.zero;
    private Vector2 _knockbackVelocity = Vector2.zero;
    private PlayerStatus _status;
    private Transform _transform;
    private readonly List<IAbility> _abilities = new();
    private readonly HashSet<EnemyStatus> _enemies = new();

    void Start()
    {
        _transform = transform;
        _status = GetComponent<PlayerStatus>();

        // TODO: Player should select which abilities they want after a wave
        _abilities.Add(new BoostAbility(_abilitiesConfig, _status));
        _abilities.Add(new SheildAbility(_abilitiesConfig, _status));
        _abilities.Add(new C4Ability(_abilitiesConfig, _status, _c4Prefab));
        _abilities.Add(new LureAbility(_abilitiesConfig, _status, _lurePrefab));

        // foreach (IAbility ability in _abilities)
        // {
        //     ability.Upgrade();
        // }
    }

    void Update()
    {
        _abilities.ForEach(ability => ability.Update());

        Vector2 _knockbackDisplacement = _knockbackVelocity * Time.deltaTime;
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction;
        _transform.position += (Vector3)(_walkingDisplacement + _knockbackDisplacement);

        if (_status.IsDead)
        {
            gameObject.SetActive(false); // Set inactive when dead
        }
    }

    public void Move(Vector2 dir)
    {
        float maxDisplacement = _status.MoveSpeed * Time.deltaTime;
        _walkingDisplacement = dir.normalized * math.min(maxDisplacement, dir.magnitude);
    }

    public void UseAbility(int number)
    {
        if (number > _abilities.Count)
        {
            Debug.Log($"Ability {number} not unlocked.");
            return;
        }
        Debug.Log($"Using Ability {number}.");
        var ability = _abilities[number - 1];
        if (!ability.Activate())
        {
            Debug.Log($"{ability.Name} is on Cooldown.");
        }
    }

    public void Attack()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPos = _transform.position;
        Vector2 targetDirection = targetPos - currentPos;
        _knockbackVelocity += _status.AttackSelfKnockforward * targetDirection.normalized;

        // Apply Damage and Knockback to enemies in range
        foreach (EnemyStatus enemy in _enemies)
        {
            Vector2 enemyDirection = (Vector2)enemy.transform.position - currentPos;
            if (enemyDirection.magnitude > _status.AttackRange || Vector2.Angle(targetDirection, enemyDirection) > _status.AttackRadius)
                continue;
            enemy.ApplyDamage(_status.AttackDamage);
            enemy.ApplyKnockbackFrom(currentPos, _status.AttackKnockback);
        }
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        Vector2 currentPosition = _transform.position;
        Vector2 direction = currentPosition - position;
        float distance = direction.magnitude;
        float inverseSquaredDistance = math.min(1 / (distance * distance), 2);
        _knockbackVelocity += inverseSquaredDistance * knockback * direction.normalized;
    }

    // Maintain a set of nearby enemies
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemies.Add(collision.GetComponent<EnemyStatus>());
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            _enemies.Remove(collider.GetComponent<EnemyStatus>());
        }
    }
}
