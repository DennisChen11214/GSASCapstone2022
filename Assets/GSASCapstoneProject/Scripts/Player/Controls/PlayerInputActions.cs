using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerInputActions : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private void Update() => FrameInput = Gather();

    private InputActionAsset _actions;
    private InputAction _move, _jump, _dash, _attack, _swap;

    private void Awake()
    {
        _actions = GetComponent<PlayerInput>().actions;
        _move = _actions.FindActionMap("Player").FindAction("Movement");
        _jump = _actions.FindActionMap("Player").FindAction("Jump");
        _dash = _actions.FindActionMap("Player").FindAction("Movement Ability");
        _attack = _actions.FindActionMap("Player").FindAction("Attack");
        _swap = _actions.FindActionMap("Player").FindAction("Swap");
    }

    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    private FrameInput Gather()
    {
        return new FrameInput
        {
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            DashDown = _dash.WasPressedThisFrame(),
            AttackDown = _attack.WasPressedThisFrame(),
            SwapDown = _swap.IsPressed(),

            Move = _move.ReadValue<Vector2>()
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool AttackDown;
    public bool SwapDown;
}
