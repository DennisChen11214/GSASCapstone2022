///
/// Created by Dennis Chen
///

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    [SerializeField]
    PlayerInput _player1Input;
    [SerializeField]
    PlayerInput _player2Input;

    // Start is called before the first frame update
    void Start()
    {
        if(Gamepad.all.Count == 1)
        {
            InputDevice[] player1Devices = { Keyboard.current};
            InputDevice[] player2Devices = { Gamepad.all[0] };
            _player1Input.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
            _player2Input.SwitchCurrentControlScheme("Controller", player2Devices);
        }
        else if (Gamepad.all.Count == 2)
        {
            InputDevice[] player1Devices = { Gamepad.all[0] };
            InputDevice[] player2Devices = { Gamepad.all[1] };
            _player1Input.SwitchCurrentControlScheme("Controller", player1Devices);
            _player2Input.SwitchCurrentControlScheme("Controller", player2Devices);
        }
        else
        {
            InputDevice[] player1Devices = { Keyboard.current };
            InputDevice[] player2Devices = { Keyboard.current };
            _player1Input.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
            _player2Input.SwitchCurrentControlScheme("KeyboardPlayer2", player2Devices);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
