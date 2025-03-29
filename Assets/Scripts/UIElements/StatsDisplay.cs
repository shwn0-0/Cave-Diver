using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    PlayerStatus _player;
    TextMeshProUGUI _text;

    void Awake()
    {
        _player = FindFirstObjectByType<PlayerStatus>();
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateWithLatest()
    {
        _text.SetText(GenerateStatsInfo());
    }

    private string GenerateStatsInfo()
    {
        if (_player == null)
            return "";

        return new StringBuilder()
            .Append("Status Effects:\n")
            .AppendJoin('\n',
                _player.StatusEffects
                    .Select(effect => $"   {effect.GetType().Name} ({TimeSpan.FromSeconds(effect.Duration):%s}s)")
                    .DefaultIfEmpty("    None")
            )
            .Append("\n\nStats:\n")
            .Append($"    Max Health: {_player.MaxHealth} (+{_player.BonusHealth})\n")
            .Append($"    Max Shield: {_player.MaxShield} (+{_player.BonusShield})\n")
            .Append($"    Attack Damage: {_player.AttackDamage} (+{_player.BonusAttackDamage})\n")
            .Append($"    Damage Multiplier : x{_player.DamageMultiplier}\n")
            .Append($"    Speed: {_player.MoveSpeed} (+{_player.BonusMoveSpeed})\n")
            .Append($"    Speed Multiplier: x{_player.MoveSpeedMultiplier}")
            .ToString();
    }
}
