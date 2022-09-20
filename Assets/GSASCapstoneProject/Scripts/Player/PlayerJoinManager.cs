using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerJoinManager : MonoBehaviour
{
    [SerializeField]
    GameObject player1Prefab;
    [SerializeField]
    GameObject player2Prefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput player1 = PlayerInput.Instantiate(player1Prefab, 0);
        PlayerInput player2 = PlayerInput.Instantiate(player2Prefab, 1);
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
