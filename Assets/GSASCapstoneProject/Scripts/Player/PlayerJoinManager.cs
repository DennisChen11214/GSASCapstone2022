using System.Collections;
using System.Collections.Generic;
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
        InputDevice[] player1Devices = { Keyboard.current};
        InputDevice[] player2Devices = { Gamepad.all[0] };
        _player1Input.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
        _player2Input.SwitchCurrentControlScheme("Controller", player2Devices);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
