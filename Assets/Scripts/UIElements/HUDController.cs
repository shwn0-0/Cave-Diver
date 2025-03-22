using System;
using System.Linq;
using UnityEngine;

class HUDController : MonoBehaviour
{
    private SliderController _shieldSlider;
    private SliderController _healthSlider;
    private AbilityController[] _abilityControllers;

    private int _numAbilities = 0; 

    void Awake()
    {
        SliderController[] sliders = GetComponentsInChildren<SliderController>();
        _abilityControllers = GetComponentsInChildren<AbilityController>().OrderBy(abilityController => abilityController.ID).ToArray();

        _shieldSlider = sliders.Where(slider => slider.ID == SliderController.SliderID.Shield).Single();
        _healthSlider = sliders.Where(slider => slider.ID == SliderController.SliderID.Health).Single();
    }

    public void SetHealth(float current, float max) =>
        _healthSlider.SetAmount(current, max);

    public void SetShield(float current, float max) =>
        _shieldSlider.SetAmount(current, max);

    public void UnlockAbility(IAbility ability) =>
        _abilityControllers[_numAbilities++].SetAbility(ability);
}