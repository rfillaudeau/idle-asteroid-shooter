using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int attackStrength { get { return GetStatValue(StatType.AttackStrength); } }
    public int attackSpeed { get { return GetStatValue(StatType.AttackSpeed); } }
    public int attackRange { get { return GetStatValue(StatType.AttackRange); } }
    public int gold { get { return _gold; } }

    public Damageable damageable { get { return _damageable; } }

    public event Action onStatUpdated;
    public event Action onGoldUpdated;

    [SerializeField] private int _gold = 0;
    [SerializeField] private List<StatValue> stats;

    [SerializeField] private GameObject _attackRangeVisualizer;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private LayerMask _attackLayers;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _healthRegenerationCooldown = 1f;

    private Damageable _damageable;

    private bool _canAttack = true;
    private bool _canRegenerateHealth = true;

    public int GetStatValue(StatType type)
    {
        if (type == StatType.MaxHealth)
        {
            return _damageable.maxHealth;
        }

        StatValue stat = stats.Find(s => s.type == type);
        if (stat == null)
        {
            return 1;
        }

        return stat.value;
    }

    public void UpgradeStat(StatType type, int upgrade)
    {
        if (type == StatType.MaxHealth)
        {
            _damageable.UpgradeMaxHealth(_damageable.maxHealth + upgrade);
        }
        else
        {
            StatValue stat = stats.Find(s => s.type == type);
            if (stat == null)
            {
                return;
            }

            stat.value += upgrade;

            if (type == StatType.AttackRange)
            {
                UpdateAttackRangeVisualizer();
            }
        }

        onStatUpdated?.Invoke();
    }

    public void SpendGold(int amount)
    {
        _gold -= amount;

        onGoldUpdated?.Invoke();
    }

    private void Awake()
    {
        _damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        Asteroid.onDestroyedWithValue += AddGold;
        _damageable.onDie += Died;
    }

    private void OnDisable()
    {
        Asteroid.onDestroyedWithValue -= AddGold;
        _damageable.onDie -= Died;
    }

    private void Start()
    {
        UpdateAttackRangeVisualizer();
    }

    private void Update()
    {
        Attack();
        RegenerateHealth();
    }

    private void UpdateAttackRangeVisualizer()
    {
        _attackRangeVisualizer.transform.localScale = new Vector3(
            attackRange * 2f * (1f / transform.localScale.x),
            attackRange * 2f * (1f / transform.localScale.x),
            1f
        );
    }

    private void AddGold(int goldValue)
    {
        _gold += goldValue;

        onGoldUpdated?.Invoke();
    }

    private void Attack()
    {
        if (!_canAttack)
        {
            return;
        }

        Asteroid closestAsteroid = GetClosestAsteroidInRange();
        if (closestAsteroid != null)
        {
            _canAttack = false;

            Vector2 direction = (closestAsteroid.transform.position - transform.position).normalized;

            Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            projectile.strength = attackStrength;
            projectile.Shoot(direction);

            StartCoroutine(AttackCooldown());
        }
    }

    private void RegenerateHealth()
    {
        int healthRegeneration = GetStatValue(StatType.HealthRegeneration);
        if (healthRegeneration == 0 || !_canRegenerateHealth)
        {
            return;
        }

        _canRegenerateHealth = false;

        _damageable.Heal(healthRegeneration);

        StartCoroutine(HealthRegenerationCooldown());
    }

    private Asteroid GetClosestAsteroidInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, _attackLayers);
        Asteroid closestAsteroid = null;
        foreach (Collider2D collider in colliders)
        {
            Asteroid asteroid = collider.GetComponent<Asteroid>();
            if (asteroid == null)
            {
                continue;
            }

            if (closestAsteroid == null)
            {
                closestAsteroid = asteroid;
                continue;
            }

            float playerToAsteroid = Vector2.Distance(transform.position, asteroid.transform.position);
            float playerToClosestAsteroid = Vector2.Distance(transform.position, closestAsteroid.transform.position);

            if (playerToAsteroid < playerToClosestAsteroid)
            {
                closestAsteroid = asteroid;
            }
        }

        return closestAsteroid;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown / attackSpeed);

        _canAttack = true;
    }

    private IEnumerator HealthRegenerationCooldown()
    {
        yield return new WaitForSeconds(_healthRegenerationCooldown);

        _canRegenerateHealth = true;
    }

    private void Died()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
