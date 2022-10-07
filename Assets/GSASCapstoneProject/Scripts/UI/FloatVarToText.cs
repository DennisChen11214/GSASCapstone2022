using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using UnityEngine.UI;
using TMPro;

public class FloatVarToText : MonoBehaviour
{
    [SerializeField]
    FloatVariable _floatVar;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if(_floatVar.Value > 0)
        {
            _text.text = Mathf.CeilToInt(_floatVar.Value).ToString();
        }
        else
        {
            _text.text = "";
        }
    }
}
