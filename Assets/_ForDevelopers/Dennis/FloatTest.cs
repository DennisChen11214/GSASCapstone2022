using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class FloatTest : MonoBehaviour
{
    public FloatVariable fVar;
    public FloatGlobalEvent fEvent;

    private void Start()
    {
        fEvent.Subscribe(EventTest);
        fVar.Subscribe(VarTest);
    }

    private void EventTest(float val)
    {
        Debug.Log("Float Event: " + val);
    }

    private void VarTest()
    {
        Debug.Log("Float Var Changed: " + fVar.Value);
    }
}
