using System.Linq;
using Unity.Mathematics;
using UnityEngine;

class HUDController : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;

    private AbilityController[] _abilityControllers;
    private CanvasGroup _canvasGroup;
    private SliderController _shieldSlider;
    private SliderController _healthSlider;
    private float _targetAlpha;
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
        _canvasGroup.alpha += math.sign(_targetAlpha - _canvasGroup.alpha) * Time.deltaTime / _fadeDuration;
        _canvasGroup.blocksRaycasts = _canvasGroup.alpha > 0.90f;
    }

    public void SetHealth(float current, float max) =>
        _healthSlider.SetAmount(current, max);

    public void SetShield(float current, float max) =>
        _shieldSlider.SetAmount(current, max);

    public void UnlockAbilitySlot(int count)
    {
        int max = math.min(count, _abilityControllers.Length - _numAbilities);
        for (int i = 0; i < max; i++)
            _abilityControllers[_numAbilities + i].SetAvailable();
    }

    public void UnlockAbility(IAbility ability)
    {
        if (_numAbilities == _abilityControllers.Length) return;
        _abilityControllers[_numAbilities++].SetAbility(ability);
    }

    public void Show() => _targetAlpha = 1f;
    public void Hide() => _targetAlpha = 0f;
}