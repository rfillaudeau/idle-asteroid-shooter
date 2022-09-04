using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health { get { return _health; } }
    public int maxHealth { get { return _maxHealth; } }
    public bool isDead { get { return _health == 0; } }
    public bool isHealthFull { get { return _health == _maxHealth; } }

    public event Action<int> onTakeDamage;
    public event Action<int> onHeal;
    public event Action onHealthChanged;
    public event Action onDie;

    private int _health = 0;
    [SerializeField] private int _maxHealth = 1;

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        _health -= damage;

        if (_health <= 0)
        {
            _health = 0;
        }

        onTakeDamage?.Invoke(damage);
        onHealthChanged?.Invoke();

        if (_health <= 0f)
        {
            onDie?.Invoke();
        }
    }

    public void Heal(int heal)
    {
        if (isHealthFull)
        {
            return;
        }

        _health += heal;

        if (_health >= _maxHealth)
        {
            _health = _maxHealth;
        }

        onHeal?.Invoke(heal);
        onHealthChanged?.Invoke();
    }

    public void Revive()
    {
        _health = _maxHealth;

        onHealthChanged?.Invoke();
    }

    public void UpgradeMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
    }

    private void Awake()
    {
        Revive();
    }
}
