using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float strength = 1f;

    [SerializeField] private float _destroyAfterSeconds = 1f;
    [SerializeField] private float _shootForce = 10f;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _explosionParticle;

    private Rigidbody2D _rigidbody;
    private Camera _camera;

    public void Shoot(Vector2 direction)
    {
        _rigidbody.AddForce(direction * _shootForce, ForceMode2D.Impulse);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(DestroyAfterSeconds());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid == null)
        {
            return;
        }

        asteroid.TakeDamage(strength);

        _spriteRenderer.gameObject.SetActive(false);
        _rigidbody.velocity = Vector2.zero;

        _explosionParticle.Play();

        Destroy(gameObject, _explosionParticle.main.duration);
    }

    private IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(_destroyAfterSeconds);

        Destroy(gameObject);
    }
}
