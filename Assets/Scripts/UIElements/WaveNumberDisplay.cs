using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

class WaveNumberDisplay : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private float t = 1f;
    Action<Color, float> _currentEffect;
    readonly float _duration = 3f;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (t >= 1f) return;
        HandleEffect();
    }

    public IEnumerator DisplayWave(int number)
    {
        _text.SetText($"Wave {number}");
        t = 0f;
        _currentEffect = FadeEffect(2f/3f);
        yield return new WaitForSeconds(_duration);
    }

    private void HandleEffect()
    {
        t += Time.deltaTime / _duration;
        _currentEffect(Color.black, t);
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