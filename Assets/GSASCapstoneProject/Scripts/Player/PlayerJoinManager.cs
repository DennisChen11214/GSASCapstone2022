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
                player2Devices[0] = Gamepad.all[0];
                _player2Input.SwitchCurrentControlScheme("Controller", player2Devices);
            }
        }
    }

    private void P1DeviceLost(PlayerInput input)
    {
        Debug.Log("Player 1 Device lost");
        InputDevice[] player1Devices = { Keyboard.current };
        _player1Input.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        /*PlayerInput playerInput;
        if(device == _player1Input.GetDevice<Gamepad>())
        {
            playerInput = _player1Input;
            Debug.Log("P1");
        }
        else
        {
            playerInput = _player2Input;
            Debug.Log("P2");
        }
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log("Device added: " + device);
                
                break;

            case InputDeviceChange.Removed:
                Debug.Log("Device removed: " + device);
                break;
        }*/
    }
    private void OnEnable()
    {
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }
}
