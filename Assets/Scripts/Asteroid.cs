using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Damageable))]
public class Asteroid : MonoBehaviour
{
    public int goldValue { get { return _goldValue; } }
    public Damageable damageable { get { return _damageable; } }

    public static event System.Action onDestroyed;
    public static event System.Action<int> onDestroyedWithValue;

    [SerializeField] private int _goldValue = 1;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private int _strength = 1;
    [SerializeField] private float _attackCooldown = 1f;

    private Rigidbody2D _rigidbody;
    private Damageable _damageable;

    private Transform _target;
    private bool _canAttack = true;

    public void MultiplyStats(int multiplier)
    {
        int randomValue = Random.Range(0, 100);

        Vector3 scale = new Vector3(0.5f, 0.5f, 0f);
        if (randomValue < 5) // 5% chance to make a big and slow asteroid
        {
            _damageable.UpgradeMaxHealth(3);

            _speed = 1f;
            _strength = 3;
            scale *= 2f;
            _goldValue = 4;
        }
        else if (randomValue >= 5 && randomValue < 15) // 10% chance to make a small and fast asteroid
        {
            _damageable.UpgradeMaxHealth(1);

            _speed = 3f;
            _strength = 1;
            scale *= 0.5f;
            _goldValue = 1;
        }
        else
        {
            _damageable.UpgradeMaxHealth(2);

            _speed = 2f;
            _strength = 2;
            _goldValue = 2;
        }

        transform.localScale = scale;

        _damageable.UpgradeMaxHealth(_damageable.maxHealth * multiplier);

        _speed *= multiplier;
        _strength *= multiplier;
        _goldValue *= multiplier;

        _damageable.Revive();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        _damageable.onDie += Died;
    }

    private void OnDisable()
    {
        _damageable.onDie -= Died;
    }

    private void Update()
    {
        LookAtTarget();
    }

    private void FixedUpdate()
    {
        MoveToTarget();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!_canAttack || !gameObject.activeInHierarchy)
        {
            return;
        }

        Player player = other.collider.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        _canAttack = false;

        player.damageable.TakeDamage(_strength);

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);

        _canAttack = true;
    }

    private void LookAtTarget()
    {
        if (_target == null)
        {
            return;
        }

        transform.up = (_target.position - transform.position).normalized;
    }

    private void MoveToTarget()
    {
        if (_target == null)
        {
            return;
        }

        _rigidbody.AddForce(transform.up * _speed);
    }

    private void Died()
    {
        onDestroyed?.Invoke();
        onDestroyedWithValue?.Invoke(_goldValue);

        gameObject.SetActive(false);
    }
}
