using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerController _controller;

    void Start()
    {
        _controller = GetComponent<PlayerController>();
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
}
