using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    [SerializeField] AbilityUIConfig _config;
    [SerializeField, Range(1, 4)] int _id;

    private TextMeshProUGUI _cooldownText;    
    private IAbility _ability;
    private Image _image;

    public int ID => _id;

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = _config.LockedColor;
        _cooldownText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (_ability == null) return;
        HandleImageColor();
        HandleCooldown();
    }

    private void HandleImageColor()
    {
        _image.color = _ability.IsAvailable ? _config.UnlockedColor : _config.CooldownColor;
    }

    private void HandleCooldown()
    {
        if (_ability.IsAvailable)
        {
            _cooldownText.enabled = false;
            return;
        }
        _cooldownText.enabled = true;
        _cooldownText.SetText(math.ceil(_ability.Cooldown).ToString());
    }

    public void SetAbility(IAbility ability)
    {
        _ability = ability;
        _image.sprite = _config.GetSprite(ability);
        _image.color = _config.UnlockedColor;
    }

    public void SetAvailable() =>
        _image.color = _config.UnlockedColor;
}
