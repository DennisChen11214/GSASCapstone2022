///
/// Created by Dennis Chen
///

using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Core.GlobalVariables;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftArrowP1;
    [SerializeField]
    private GameObject _rightArrowP1;
    [SerializeField]
    private TMP_Text _controlsTextP1;
    [SerializeField]
    private GameObject _leftArrowP2;
    [SerializeField]
    private GameObject _rightArrowP2;
    [SerializeField]
    private TMP_Text _controlsTextP2;
    [SerializeField]
    private StringVariable _controlsP1;
    [SerializeField]
    private StringVariable _controlsP2;

    private int _numGamepads;

    private void Start()
    {
        _numGamepads = Gamepad.all.Count;
        if(_numGamepads == 0)
        {
            _rightArrowP1.SetActive(false);
            _rightArrowP2.SetActive(false);
        }
    }

    //Changes from controller to keyboard for player 1
    public void PreviousP1()
    {
        _leftArrowP1.SetActive(false);
        _rightArrowP1.SetActive(true);
        _controlsTextP1.text = "Keyboard";
        _numGamepads++;
        if(_numGamepads == 1 && _controlsTextP2.text != "Controller")
        {
            _rightArrowP2.SetActive(true);
        }
    }

    //Changes from keyboard to controller for player 1
    public void NextP1()
    {
        _leftArrowP1.SetActive(true);
        _rightArrowP1.SetActive(false);
        _controlsTextP1.text = "Controller";
        _numGamepads--;
        if (_numGamepads == 0)
        {
            _rightArrowP2.SetActive(false);
        }
    }

    //Changes from controller to keyboard for player 2
    public void PreviousP2()
    {
        _leftArrowP2.SetActive(false);
        _rightArrowP2.SetActive(true);
        _controlsTextP2.text = "Keyboard";
        _numGamepads++;
        if (_numGamepads == 1 && _controlsTextP1.text != "Controller")
        {
            _rightArrowP1.SetActive(true);
        }
    }

    //Changes from keyboard to controller for player 2
    public void NextP2()
    {
        _leftArrowP2.SetActive(true);
        _rightArrowP2.SetActive(false);
        _controlsTextP2.text = "Controller";
        _numGamepads--;
        if (_numGamepads == 0)
        {
            _rightArrowP1.SetActive(false);
        }
    }

    //Stores which controls each player is using and goes to the next scene
    public void StartGame()
    {
        _controlsP1.Value = _controlsTextP1.text;
        _controlsP2.Value = _controlsTextP2.text;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Handles what happens if a controller is added or removed while in this scene
    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                _numGamepads++;
                Debug.Log("New device added: " + _numGamepads);
                if (_controlsTextP1.text != "Controller")
                {
                    _rightArrowP1.SetActive(true);
                }
                if (_controlsTextP2.text != "Controller")
                {
                    _rightArrowP2.SetActive(true);
                }
                break;

            case InputDeviceChange.Removed:
                Debug.Log("Device removed: " + Gamepad.all.Count);
                _rightArrowP1.SetActive(false);
                _rightArrowP2.SetActive(false);
                _leftArrowP1.SetActive(false);
                _leftArrowP2.SetActive(false);
                if(Gamepad.all.Count == 1)
                {
                    if(_controlsTextP1.text == "Controller" && _controlsTextP2.text == "Controller")
                    {
                        _leftArrowP2.SetActive(true);
                        _controlsTextP1.text = "Keyboard";
                    }
                    else if(_controlsTextP1.text == "Controller")
                    {
                        _leftArrowP1.SetActive(true);
                    }
                    else if(_controlsTextP2.text == "Controller")
                    {
                        _leftArrowP2.SetActive(true);
                    }
                    else
                    {
                        _numGamepads--;
                    }
                }
                else if (Gamepad.all.Count == 0)
                {
                    _controlsTextP1.text = "Keyboard";
                    _controlsTextP2.text = "Keyboard";
                }
                break;
        }
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
