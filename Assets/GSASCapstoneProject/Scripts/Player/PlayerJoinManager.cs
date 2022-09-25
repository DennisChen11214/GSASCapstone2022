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
    [SerializeField]
    Transform _player1Spawn;
    [SerializeField]
    Transform _player2Spawn;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput player1 = PlayerInput.Instantiate(_player1Prefab, 0);
        player1.transform.position = _player1Spawn.position;
        player1.transform.parent.parent = _player1Spawn.transform.parent;
        PlayerInput player2 = PlayerInput.Instantiate(_player2Prefab, 1);
        player2.transform.position = _player2Spawn.position;
        player2.transform.parent.parent = _player2Spawn.transform.parent;
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
