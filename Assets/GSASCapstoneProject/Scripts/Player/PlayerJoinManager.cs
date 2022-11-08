///
/// Created by Dennis Chen
///

using Core.GlobalVariables;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    [SerializeField]
    PlayerInput _player1Input;
    [SerializeField]
    PlayerInput _player2Input;
    [SerializeField]
    StringVariable _p1Device;
    [SerializeField]
    StringVariable _p2Device;

    // Start is called before the first frame update
    void Start()
    {
        InputDevice[] player1Devices = new InputDevice[1];
        InputDevice[] player2Devices = new InputDevice[1];
        if (_p1Device.Value == "" && _p2Device.Value == "")
        {
            player1Devices[0] = Keyboard.current;
            _player1Input.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
            player2Devices[0] = Keyboard.current;
            _player2Input.SwitchCurrentControlScheme("KeyboardPlayer2", player2Devices);
            return;
        }
        if (_p1Device.Value == "Controller" && _p2Device.Value == "Controller")
        {
            InputSystem.EnableDevice(Gamepad.all[0]);
            InputSystem.EnableDevice(Gamepad.all[1]);
            player1Devices[0] = Gamepad.all[0];
            _player1Input.SwitchCurrentControlScheme("Controller", player1Devices);
            player2Devices[0] = Gamepad.all[1];
            _player2Input.SwitchCurrentControlScheme("Controller", player2Devices);
        }
        else
        {
            if(_p1Device.Value == "Keyboard")
            {
                player1Devices[0] = Keyboard.current;
                _player1Input.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
            }
            else
            {
                InputSystem.EnableDevice(Gamepad.all[0]);
                player1Devices[0] = Gamepad.all[0];
                _player1Input.SwitchCurrentControlScheme("Controller", player1Devices);
            }
            if (_p2Device.Value == "Keyboard")
            {
                player2Devices[0] = Keyboard.current;
                _player2Input.SwitchCurrentControlScheme("KeyboardPlayer2", player2Devices);
            }
            else
            {
                InputSystem.EnableDevice(Gamepad.all[0]);
                player2Devices[0] = Gamepad.all[0];
                _player2Input.SwitchCurrentControlScheme("Controller", player2Devices);
            }
        }
    }
}
