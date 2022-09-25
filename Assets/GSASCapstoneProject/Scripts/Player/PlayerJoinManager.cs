using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerJoinManager : MonoBehaviour
{
    [SerializeField]
    GameObject _player1Prefab;
    [SerializeField]
    GameObject _player2Prefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput player1 = PlayerInput.Instantiate(_player1Prefab, 0);
        PlayerInput player2 = PlayerInput.Instantiate(_player2Prefab, 1);
        InputDevice[] player1Devices = { Keyboard.current};
        InputDevice[] player2Devices = { Gamepad.all[0] };
        player1.SwitchCurrentControlScheme("KeyboardPlayer1", player1Devices);
        player2.SwitchCurrentControlScheme("Controller", player2Devices);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
