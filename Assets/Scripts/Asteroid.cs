using System.Collections;
using UnityEngine;

public class Asteroid : Damageable
{
    public int goldValue { get { return _goldValue; } }

    public static event System.Action onDestroyed;
    public static event System.Action<int> onDestroyedWithValue;

    [SerializeField] private int _goldValue = 1;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _strength = 1f;
    [SerializeField] private float _attackCooldown = 1f;

    [SerializeField] private DamageDisplayer _damageDisplayerPrefab;

    private Transform _target;
    private bool _canAttack = true;

    public void MultiplyStats(float multiplier)
    {
        int randomValue = Random.Range(0, 100);

        Vector3 scale = new Vector3(0.5f, 0.5f, 0f);
        if (randomValue < 5) // 5% chance to make a big and slow asteroid
        {
            _maxHealth = 2f;
            _speed = 0.5f;
            _strength = 2f;
            scale *= 2f;
            _goldValue = 4;
        }
        else if (randomValue >= 5 && randomValue < 15) // 10% chance to make a small and fast asteroid
        {
            _maxHealth = 0.5f;
            _speed = 2f;
            _strength = 0.5f;
            scale *= 0.5f;
            _goldValue = 1;
        }
        else
        {
            _maxHealth = 1f;
            _speed = 1f;
            _strength = 1f;
            _goldValue = 2;
        }

        transform.localScale = scale;

        _maxHealth *= multiplier;
        _speed *= multiplier;
        _strength *= multiplier;

        int intMultiplier = Mathf.RoundToInt(multiplier);
        if (intMultiplier > 1)
        {
            _goldValue *= intMultiplier;
        }

        ResetHealth();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        MoveToTarget();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        DamageDisplayer damageDisplayer = Instantiate(_damageDisplayerPrefab, transform.position, Quaternion.identity);
        damageDisplayer.transform.localScale *= 0.5f;
        damageDisplayer.SetDamageColor(Color.red);
        damageDisplayer.SetDamageText(damage.ToString("0.00"));

        if (health <= 0f)
        {
            onDestroyed?.Invoke();
            onDestroyedWithValue?.Invoke(_goldValue);

            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_canAttack)
        {
            return;
        }

        Player player = other.collider.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        _canAttack = false;

        player.TakeDamage(_strength);

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);

        _canAttack = true;
    }

    private void MoveToTarget()
    {
        if (_target == null)
        {
            return;
        }

        // Vector2 direction = (_target.position - transform.position).normalized;
        // transform.Translate(direction * Time.deltaTime * _speed, Space.World);
        transform.up = (_target.position - transform.position).normalized;
        transform.Translate(Vector2.up * Time.deltaTime * _speed);
    }
}
