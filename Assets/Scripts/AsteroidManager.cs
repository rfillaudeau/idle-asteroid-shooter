using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    public int waveCount { get { return _waveCount; } }

    public event System.Action onNewWave;

    [SerializeField] private Asteroid _asteroidPrefab;

    [SerializeField] private int _asteroidStatsMultiplier = 1;
    [SerializeField] private float _spawnPositionOffset = 1f;
    [SerializeField] private float _spawnCooldown = 1f;

    [SerializeField] private Transform _target;
    [SerializeField] private int _asteroidsPerWave = 20;
    [SerializeField] private int _waveCount = 0;

    private int _spawnedAsteroidsCount = 0;
    private int _destroyedAsteroidsCount = 0;

    private List<Asteroid> _pool;

    private bool _canSpawn = true;

    private float _spawnXMin;
    private float _spawnXMax;
    private float _spawnYMin;
    private float _spawnYMax;

    private void OnEnable()
    {
        Asteroid.onDestroyed += OnAsteroidDestroyed;
    }

    private void OnDisable()
    {
        Asteroid.onDestroyed -= OnAsteroidDestroyed;
    }

    private void Awake()
    {
        Camera camera = Camera.main;

        _spawnXMin = camera.ScreenToWorldPoint(new Vector3(camera.pixelRect.xMin, 0f, 0f)).x - _spawnPositionOffset;
        _spawnXMax = camera.ScreenToWorldPoint(new Vector3(camera.pixelRect.xMax, 0f, 0f)).x + _spawnPositionOffset;
        _spawnYMin = camera.ScreenToWorldPoint(new Vector3(0f, camera.pixelRect.yMin, 0f)).y - _spawnPositionOffset;
        _spawnYMax = camera.ScreenToWorldPoint(new Vector3(0f, camera.pixelRect.yMax, 0f)).y + _spawnPositionOffset;
    }

    private void Start()
    {
        _pool = new List<Asteroid>();
        for (int i = 0; i < _asteroidsPerWave; i++)
        {
            Asteroid asteroid = Instantiate(_asteroidPrefab, Vector3.zero, Quaternion.identity);
            asteroid.gameObject.SetActive(false);

            _pool.Add(asteroid);
        }

        _waveCount++;
    }

    private void Update()
    {
        SpawnAsteroid();
    }

    public Asteroid GetPooledAsteroid()
    {
        foreach (Asteroid asteroid in _pool)
        {
            if (!asteroid.gameObject.activeInHierarchy)
            {
                return asteroid;
            }
        }

        return null;
    }

    private void SpawnAsteroid()
    {
        if (!_canSpawn)
        {
            return;
        }

        _canSpawn = false;
        StartCoroutine(SpawnCooldown());

        if (_destroyedAsteroidsCount == _asteroidsPerWave)
        {
            SetupNewWave();
            return;
        }

        if (_spawnedAsteroidsCount >= _asteroidsPerWave)
        {
            return;
        }

        Asteroid asteroid = GetPooledAsteroid();
        if (asteroid == null)
        {
            return;
        }

        asteroid.transform.position = GenerateRandomPosition();
        asteroid.SetTarget(_target);
        asteroid.MultiplyStats(_asteroidStatsMultiplier);
        asteroid.gameObject.SetActive(true);

        _spawnedAsteroidsCount++;
    }

    private void SetupNewWave()
    {
        _waveCount++;
        _asteroidStatsMultiplier++;
        _spawnedAsteroidsCount = 0;
        _destroyedAsteroidsCount = 0;

        onNewWave?.Invoke();
    }

    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(_spawnCooldown / _asteroidStatsMultiplier);

        _canSpawn = true;
    }

    private void OnAsteroidDestroyed()
    {
        _destroyedAsteroidsCount++;
    }

    private Vector2 GenerateRandomPosition()
    {
        Vector2 position = Vector2.zero;

        if (Random.value >= 0.5f)
        {
            position.x = Random.Range(_spawnXMin, _spawnXMax);
            position.y = Random.value >= 0.5f ? _spawnYMin : _spawnYMax;
        }
        else
        {
            position.x = Random.value >= 0.5f ? _spawnXMin : _spawnXMax;
            position.y = Random.Range(_spawnYMin, _spawnYMax);
        }

        return position;
    }
}
