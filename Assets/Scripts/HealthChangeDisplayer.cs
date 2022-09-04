using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class HealthChangeDisplayer : MonoBehaviour
{
    [SerializeField] private MovingText _movingTextPrefab;

    private Damageable _damageable;

    private void Awake()
    {
        _damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        _damageable.onTakeDamage += DisplayDamage;
        _damageable.onHeal += DisplayHeal;
    }

    private void OnDisable()
    {
        _damageable.onTakeDamage -= DisplayDamage;
        _damageable.onHeal -= DisplayHeal;
    }

    private void DisplayDamage(int damage)
    {
        MovingText damageDisplayer = Instantiate(
            _movingTextPrefab,
            _damageable.transform.position,
            Quaternion.identity
        );

        damageDisplayer.SetDamageColor(Color.red);
        damageDisplayer.SetValue(damage);
    }

    private void DisplayHeal(int heal)
    {
        MovingText damageDisplayer = Instantiate(
            _movingTextPrefab,
            _damageable.transform.position,
            Quaternion.identity
        );

        damageDisplayer.SetDamageColor(Color.green);
        damageDisplayer.SetValue(heal);
    }
}
