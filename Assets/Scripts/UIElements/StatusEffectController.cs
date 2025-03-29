using UnityEngine;
using UnityEngine.UI;

class StatusEffectController : MonoBehaviour
{
    Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetStatusEffect (Sprite sprite)
    {
        gameObject.SetActive(true);
        _image.sprite = sprite;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}