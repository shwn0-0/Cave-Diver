using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

class WaveNumberDisplay : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Color _color;
    private float t = 1f;
    Func<float, float> _currentEffect;
    readonly float _duration = 3f;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _color = _text.color;
    }

    void Update()
    {
        if (t >= 1f) return;
        t += Time.deltaTime / _duration;

        _color.a = _currentEffect(t);
        _text.color = _color;
    }

    public IEnumerator DisplayWave(int number, bool IsFinalWave)
    {
        if (IsFinalWave)
        {
            _text.SetText($"Final Wave");
            _currentEffect = CombinedEffects(FadeEffect(2f/3f), FlashingEffect(4));
        }
        else
        {
            _text.SetText($"Wave {number}");
            _currentEffect = FadeEffect(2f / 3f);
        }

        t = 0f;
        yield return new WaitForSeconds(_duration);
    }

    public IEnumerator DisplayPlayerDead()
    {
        _text.SetText($"You Died!");
        t = 0f;
        _currentEffect = NoEffect;
        yield return null;
    }

    public IEnumerator DisplayPlayerWon()
    {
        _text.SetText($"You Won!");
        t = 0f;
        _currentEffect = NoEffect;
        yield return null;
    }

    private float NoEffect(float t) => 1f;

    private Func<float, float> CombinedEffects(params Func<float, float>[] effects) =>
        (t) => effects.Select(effect => effect(t)).Aggregate(1f, (curr, next) => curr * next);

    private Func<float, float> FlashingEffect(float freq) =>
        (t) => 0.5f * math.cos(t * math.PI2 * freq) + 0.5f;

    private Func<float, float> FadeEffect(float delay)
    {
        return (t) =>
        {
            float a = math.step(1 - delay, 1 - t); // Hold at 1 for delay seconds
            a += math.step(delay, t) * math.lerp(1, 0, (t - delay) * (1 / (1 - delay))); // Lerp between 1 and 0 after delay seconds
            return a;
        };
    }
}