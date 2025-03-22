using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController _controller;
    private WaveController _waveController;

    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _waveController = FindFirstObjectByType<WaveController>();
    }

    void OnMove(InputValue input) =>
        _controller.Move(input.Get<Vector2>());

    void OnAttack(InputValue input)
    {
        if (!input.isPressed) return;
        _controller.Attack();
    }

    void OnAbility1(InputValue input) => OnAbility(input, 1);
    void OnAbility2(InputValue input) => OnAbility(input, 2);
    void OnAbility3(InputValue input) => OnAbility(input, 3);
    void OnAbility4(InputValue input) => OnAbility(input, 4);

    void OnAbility(InputValue input, int number)
    {
        if (!input.isPressed) return;
        _controller.UseAbility(number);
    }

    void OnPause(InputValue input)
    {
        if (!input.isPressed) return;
        _waveController.TogglePause();
    }
}
