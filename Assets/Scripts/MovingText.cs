using UnityEngine;
using TMPro;

public class MovingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _damageText;

    public void SetDamageText(string text)
    {
        _damageText.SetText(text);
    }

    public void SetValue(int damage)
    {
        _damageText.SetText(damage.ToString());
    }

    public void SetValue(float damage)
    {
        _damageText.SetText(damage.ToString("0.0"));
    }

    public void SetDamageColor(Color color)
    {
        _damageText.color = color;
    }
}
