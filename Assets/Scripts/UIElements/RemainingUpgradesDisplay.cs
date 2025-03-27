using TMPro;
using UnityEngine;

public class RemainingUpgradesDisplay : MonoBehaviour
{
    TextMeshProUGUI _text;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();         
    }

    public void SetRemainingUpgrades (int count)
    {
        _text.SetText($"Remaining Upgrades: {count}");
    }
}
