using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private Player _player;

    [SerializeField] private Button _attackStrengthButton;
    [SerializeField] private Button _attackSpeedButton;
    [SerializeField] private Button _attackRangeButton;
    [SerializeField] private Button _maxHealthButton;
    [SerializeField] private Button _healthRegenerationButton;

    [SerializeField] private List<StatUpgrade> _statUpgrades = new List<StatUpgrade>();

    private void OnEnable()
    {
        _player.onGoldUpdated += UpdateUpgradeButtons;
        _player.damageable.onDie += DisableButtons;
    }

    private void OnDisable()
    {
        _player.onGoldUpdated -= UpdateUpgradeButtons;
        _player.damageable.onDie -= DisableButtons;
    }

    private void Awake()
    {
        _attackStrengthButton.onClick.AddListener(() => UpgradeStat(StatType.AttackStrength));
        _attackSpeedButton.onClick.AddListener(() => UpgradeStat(StatType.AttackSpeed));
        _attackRangeButton.onClick.AddListener(() => UpgradeStat(StatType.AttackRange));
        _maxHealthButton.onClick.AddListener(() => UpgradeStat(StatType.MaxHealth));
        _healthRegenerationButton.onClick.AddListener(() => UpgradeStat(StatType.HealthRegeneration));
    }

    private void Start()
    {
        UpdateUpgradeButtons();
    }

    private void UpgradeStat(StatType type)
    {
        StatUpgrade upgrade = _statUpgrades.Find(u => u.type == type);
        if (upgrade == null)
        {
            return;
        }

        _player.UpgradeStat(type, upgrade.value);
        _player.SpendGold(upgrade.cost);

        upgrade.cost += upgrade.incrementalCost;

        UpdateUpgradeButtons();
    }

    private void UpdateUpgradeButtons()
    {
        UpdateButton(_attackStrengthButton, StatType.AttackStrength);
        UpdateButton(_attackSpeedButton, StatType.AttackSpeed);
        UpdateButton(_attackRangeButton, StatType.AttackRange);
        UpdateButton(_maxHealthButton, StatType.MaxHealth);
        UpdateButton(_healthRegenerationButton, StatType.HealthRegeneration);
    }

    private void DisableButtons()
    {
        _attackStrengthButton.interactable = false;
        _attackSpeedButton.interactable = false;
        _attackRangeButton.interactable = false;
        _maxHealthButton.interactable = false;
        _healthRegenerationButton.interactable = false;
    }

    private void UpdateButton(Button button, StatType type)
    {
        StatUpgrade upgrade = _statUpgrades.Find(u => u.type == type);
        if (upgrade == null)
        {
            return;
        }

        if (upgrade.hasMaxValue && _player.GetStatValue(type) >= upgrade.maxValue)
        {
            button.interactable = false;
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("MAX.");

            return;
        }

        button.interactable = _player.gold >= upgrade.cost;
        button.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("+{0} ({1} gold)", upgrade.value, upgrade.cost);
    }
}
