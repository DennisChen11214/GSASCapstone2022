using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class GlobalTest : MonoBehaviour
{
    public StringVariable sVar;
    public FloatGlobalEvent fEvent;

    private void Start()
    {
        fEvent.Subscribe(FloatTest);
    }

    private void FloatTest(float val)
    {
        Debug.Log("From Global: " + val);
    }

    private void StringTest()
    {
        Debug.Log("String: " + sVar.Value);
        sVar.Value = "Test2";
        Debug.Log("String: " + sVar.Value);
    }
}
