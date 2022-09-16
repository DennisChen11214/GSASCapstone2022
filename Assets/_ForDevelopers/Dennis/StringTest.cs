using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class StringTest : MonoBehaviour
{
    public StringVariable sVar;
    public StringGlobalEvent sEvent;

    private void Start()
    {
        Debug.Log("String: " + sVar.Value);
        sVar.Value = "Testing";
        Debug.Log("String: " + sVar.Value);
    }
}
