using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftArrow;
    [SerializeField]
    private GameObject _rightArrow;
    [SerializeField]
    private GameObject _controlsText;

    public void Previous()
    {
        _leftArrow.SetActive(false);
        _rightArrow.SetActive(true);
        _controlsText.GetComponent<TMP_Text>().text = "Keyboard";
    }

    public void Next()
    {
        _leftArrow.SetActive(true);
        _rightArrow.SetActive(false);
        _controlsText.GetComponent<TMP_Text>().text = "Controller";
    }
}
