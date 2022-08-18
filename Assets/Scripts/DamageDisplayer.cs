using UnityEngine;
using TMPro;

public class DamageDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro _damageText;

    public void SetDamageText(string text)
    {
        _damageText.SetText(text);
    }

    public void SetDamageColor(Color color)
    {
        _damageText.color = color;
    }
}
