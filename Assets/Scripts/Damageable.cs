using System;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public float health { get { return _health; } }
    public event Action onHealthChanged;
    public event Action onDie;

    [SerializeField] protected float _health = 0f;
    [SerializeField] protected float _maxHealth = 1f;

    public virtual void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0f)
        {
            _health = 0f;
        }

        onHealthChanged?.Invoke();

        if (_health <= 0f)
        {
            onDie?.Invoke();
        }
    }

    public virtual void Heal(float heal)
    {
        _health += heal;

        if (_health >= _maxHealth)
        {
            _health = _maxHealth;
        }

        onHealthChanged?.Invoke();
    }

    protected virtual void Awake()
    {
        ResetHealth();
    }

    protected void ResetHealth()
    {
        _health = _maxHealth;
    }
}
