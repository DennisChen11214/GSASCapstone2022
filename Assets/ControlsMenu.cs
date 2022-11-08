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
            _leftArrowP1.SetActive(false);
            _leftArrowP2.SetActive(false);
            _rightArrowP1.SetActive(false);
            _rightArrowP2.SetActive(false);
        }
        else if(_numGamepads == 1)
        {
            _rightArrowP1.SetActive(true);
            _leftArrowP2.SetActive(true);
            _controlsTextP2.text = "Controller";
        }
        else
        {
            _leftArrowP1.SetActive(false);
            _leftArrowP2.SetActive(false);
            _rightArrowP1.SetActive(false);
            _rightArrowP2.SetActive(false);
            _controlsTextP1.text = "Controller";
            _controlsTextP2.text = "Controller";
        }
    }

    public void PreviousP1()
    {
        _leftArrowP1.SetActive(false);
        _rightArrowP1.SetActive(true);
        _controlsTextP1.text = "Keyboard";
        _numGamepads++;
        if(_numGamepads == 1)
        {
            _rightArrowP2.SetActive(true);
        }
    }

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

    public void PreviousP2()
    {
        _leftArrowP2.SetActive(false);
        _rightArrowP2.SetActive(true);
        _controlsTextP2.text = "Keyboard";
        _numGamepads++;
        if (_numGamepads == 1)
        {
            _rightArrowP1.SetActive(true);
        }
    }

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

    public void StartGame()
    {
        _controlsP1.Value = _controlsTextP1.text;
        _controlsP2.Value = _controlsTextP2.text;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
