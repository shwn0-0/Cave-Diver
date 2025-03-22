using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

class WaveNumberDisplay : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private float t;
    Action<Color, float> _currentEffect;
    readonly float _duration = 3f;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        HandleEffect();
    }

    public void DisplayWave(int number)
    {
        gameObject.SetActive(true);
        _text.SetText($"Wave {number}");
        t = 0f;
        _currentEffect = FadeEffect(2f/3f);
    }

    private void HandleEffect()
    {
        t += Time.deltaTime / _duration;
        _currentEffect(Color.black, t);

        if (t >= 1f)
        {
            gameObject.SetActive(false);
        }
    }

    private Action<Color, float> FadeEffect(float delay)
    {
        return (color, t) => {
            float a = math.step(1 - delay, 1 - t); // Hold at 1 for delay seconds
            a += math.step(delay, t) * math.lerp(1, 0, (t - delay) * (1/(1 - delay))); // Lerp between 1 and 0 after delay seconds
            color.a = a;
            _text.color = color;
        };
    }
}