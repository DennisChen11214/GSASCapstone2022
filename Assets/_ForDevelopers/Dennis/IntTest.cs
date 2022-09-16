using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class IntTest : MonoBehaviour
{
    public IntVariable bVar;
    public IntGlobalEvent bEvent;

    private void Start()
    {
        Debug.Log("Int: " + bVar.Value);
        bVar.Value = 123;
        Debug.Log("Int: " + bVar.Value);
    }
}
