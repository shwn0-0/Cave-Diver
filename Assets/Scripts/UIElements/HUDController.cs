using System.Linq;
using UnityEngine;

class HUDController : MonoBehaviour
{
    private SliderController _shieldSlider;
    private SliderController _healthSlider;

    void Awake()
    {
        SliderController[] sliders = GetComponentsInChildren<SliderController>();

        _shieldSlider = sliders.Where(slider => slider.ID == SliderController.SliderID.Shield).Single();
        _healthSlider = sliders.Where(slider => slider.ID == SliderController.SliderID.Health).Single();
    }

    public void SetHealthPercent(float percent) =>
        _healthSlider.SetWidthPercent(percent);

    public void SetShieldPercent(float percent) =>
        _shieldSlider.SetWidthPercent(percent);
}