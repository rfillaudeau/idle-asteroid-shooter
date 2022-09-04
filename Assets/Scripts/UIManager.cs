using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerStatsText;
    [SerializeField] private TextMeshProUGUI _waveCountText;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    [SerializeField] private Button _restartButton;

    [SerializeField] private Player _player;
    [SerializeField] private AsteroidManager _asteroidManager;

    private void OnEnable()
    {
        _player.damageable.onHealthChanged += UpdatePlayerStatsText;
        _player.onStatUpdated += UpdatePlayerStatsText;
        _player.onGoldUpdated += UpdatePlayerStatsText;
        _player.damageable.onDie += DisplayGameOver;
        _asteroidManager.onNewWave += UpdateWaveCountText;
    }

    private void OnDisable()
    {
        _player.damageable.onHealthChanged -= UpdatePlayerStatsText;
        _player.onStatUpdated -= UpdatePlayerStatsText;
        _player.onGoldUpdated -= UpdatePlayerStatsText;
        _player.damageable.onDie -= DisplayGameOver;
        _asteroidManager.onNewWave -= UpdateWaveCountText;
    }

    private void Start()
    {
        UpdatePlayerStatsText();
        UpdateWaveCountText();
    }

    private void UpdatePlayerStatsText()
    {
        StringBuilder text = new StringBuilder();

        text.Append($"Gold: {_player.gold}");
        text.AppendLine();
        text.Append($"Health: {_player.damageable.health.ToString()}/{_player.damageable.maxHealth.ToString()}");
        text.AppendLine();
        text.Append($"Attack Strength: {_player.attackStrength.ToString()}");
        text.AppendLine();
        text.Append($"Attack Speed: {_player.attackSpeed.ToString()}");
        text.AppendLine();
        text.Append($"Attack Range: {_player.attackRange.ToString()}");
        text.AppendLine();
        text.Append($"Health Regen.: +{_player.GetStatValue(StatType.HealthRegeneration).ToString()}/s.");

        _playerStatsText.SetText(text.ToString());
    }

    private void UpdateWaveCountText()
    {
        _waveCountText.SetText(_asteroidManager.waveCount.ToString());
    }

    private void DisplayGameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartButton.gameObject.SetActive(true);
    }
}
