using System.Linq;
using Unity.Mathematics;
using UnityEngine;

class HUDController : MonoBehaviour
{
    private AbilityController[] _abilityControllers;
    private CanvasGroup _canvasGroup;
    private SliderController _shieldSlider;
    private SliderController _healthSlider;
    private float _targetAlpha;
    private float _duration = 0.5f;
    private int _numAbilities = 0; 

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SliderController[] sliders = GetComponentsInChildren<SliderController>();
        _abilityControllers = GetComponentsInChildren<AbilityController>().OrderBy(abilityController => abilityController.ID).ToArray();

        _shieldSlider = sliders.Where(slider => slider.ID == SliderController.SliderID.Shield).Single();
        _healthSlider = sliders.Where(slider => slider.ID == SliderController.SliderID.Health).Single();
    }

    void Update()
    {
        if (math.abs(_targetAlpha - _canvasGroup.alpha) <= float.Epsilon) return;
        _canvasGroup.alpha += math.sign(_targetAlpha - _canvasGroup.alpha) * Time.deltaTime / _duration;
        _canvasGroup.blocksRaycasts = _canvasGroup.alpha > 0.90f;
    }

    public void SetHealth(float current, float max) =>
        _healthSlider.SetAmount(current, max);

    public void SetShield(float current, float max) =>
        _shieldSlider.SetAmount(current, max);

    public void UnlockAbilitySlot() =>
        _abilityControllers[_numAbilities].SetAvailable();

    public void UnlockAbility(IAbility ability) =>
        _abilityControllers[_numAbilities++].SetAbility(ability);

    public void Show() => _targetAlpha = 1f;
    public void Hide() => _targetAlpha = 0f;
}