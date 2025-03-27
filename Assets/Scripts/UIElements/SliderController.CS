using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

class SliderController : MonoBehaviour
{
    [SerializeField] SliderID _id;
    [SerializeField] Color start;
    [SerializeField] Color end;

    public enum SliderID
    {
        Shield,
        Health
    }

    RectTransform _transform;
    Image _image;
    TextMeshProUGUI _amountText;
    float _maxWidth;
    float _height;
    float _currentPercent = 1f;
    float _targetPercent = 1f;

    public SliderID ID => _id;

    void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        var rect = _transform.rect;
        _maxWidth = rect.width;
        _height = rect.height;
    }

    void Update()
    {
        float difference = _targetPercent - _currentPercent;
        if (math.abs(difference) >= float.Epsilon)
        {
            _currentPercent += math.min(math.sign(difference) * Time.unscaledDeltaTime, difference);
            HandleWidthPercent();
        }
    }

    private void HandleWidthPercent()
    {
        _transform.sizeDelta = new(_maxWidth * _currentPercent, _height);
        float t = _currentPercent * math.step(0.1f, _currentPercent); // set t to 0 if below 10% so bar is red at 10%
        _image.color = Color.HSVToRGB(ColorHueLerp(end, start, t), 1.0f, 1.0f);
    }

    public void SetAmount(float current, float max)
    {
        _amountText.SetText($"{math.round(current)} / {math.round(max)}");
        float percent = current / max;
        _targetPercent = math.round(100f * percent) / 100f; // round to nearest percent
    }

    private float ColorHueLerp(Color a, Color b, float t)
    {
        t = math.clamp(t, 0f, 1f);
        Color.RGBToHSV(a, out float a_hue, out _, out _);
        Color.RGBToHSV(b, out float b_hue, out _, out _);
        return (a_hue * (1.0f - t)) + b_hue * t;
    }
}