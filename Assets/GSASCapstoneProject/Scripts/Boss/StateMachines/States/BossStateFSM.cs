using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossStateFSM : MonoBehaviour
{
    private iState currentState;
    [SerializeField] GameObject target;

    private Dictionary<string, iState> states = new Dictionary<string, iState>();

    private void Start()
    {
        states.Add("Idle", new BossIdle(this));
    }
}
