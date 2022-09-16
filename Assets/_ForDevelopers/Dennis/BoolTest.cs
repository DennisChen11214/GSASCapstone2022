using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class BoolTest : MonoBehaviour
{
    public BoolVariable bVar;
    public BoolGlobalEvent bEvent;

    private void Start()
    {
        bEvent.Subscribe(EventTest);
        bVar.Subscribe(VarTest);
    }

    private void EventTest(bool val)
    {
        
    }

    private void VarTest()
    {

    }
}
