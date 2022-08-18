using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Damageable
{
    public float attackStrength { get { return GetStatValue(StatType.AttackStrength); } }
    public float attackSpeed { get { return GetStatValue(StatType.AttackSpeed); } }
    public float attackRange { get { return GetStatValue(StatType.AttackRange); } }
    public float maxHealth { get { return _maxHealth; } }
    public int gold { get { return _gold; } }

    public event Action onStatUpdated;
    public event Action onGoldUpdated;

    [SerializeField] private int _gold = 0;
    [SerializeField] private List<StatValue> stats;

    [SerializeField] private DamageDisplayer _damageDisplayerPrefab;

    [SerializeField] private GameObject _attackRangeVisualizer;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private LayerMask _attackLayers;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _healthRegenerationCooldown = 1f;

    private bool _canAttack = true;
    private bool _canRegenerateHealth = true;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        DamageDisplayer damageDisplayer = Instantiate(_damageDisplayerPrefab, transform.position, Quaternion.identity);
        damageDisplayer.transform.localScale *= 0.5f;
        damageDisplayer.SetDamageColor(Color.white);
        damageDisplayer.SetDamageText(damage.ToString("0.00"));

        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public float GetStatValue(StatType type)
    {
        if (type == StatType.MaxHealth)
        {
            return _maxHealth;
        }

        StatValue stat = stats.Find(s => s.type == type);
        if (stat == null)
        {
            return 1f;
        }

        return stat.value;
    }

    public void UpgradeStat(StatType type, float upgrade)
    {
        if (type == StatType.MaxHealth)
        {
            _maxHealth += upgrade;
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

    private void OnEnable()
    {
        Asteroid.onDestroyedWithValue += AddGold;
    }

    private void OnDisable()
    {
        Asteroid.onDestroyedWithValue -= AddGold;
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
        float healthRegeneration = GetStatValue(StatType.HealthRegeneration);
        if (healthRegeneration == 0f || !_canRegenerateHealth)
        {
            return;
        }

        _canRegenerateHealth = false;

        Heal(healthRegeneration);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
