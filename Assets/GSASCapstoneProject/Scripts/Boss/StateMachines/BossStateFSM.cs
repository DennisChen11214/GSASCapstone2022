using System;
using System.Collections;
using System.Collections.Generic;
using Core.GlobalVariables;
using Unity.VisualScripting;
using UnityEngine;



public enum BossStateType
{
    Idle, Attack 
}

public class BossStateFSM: MonoBehaviour
{
    
    public TransformVariable target;
    
    // modules attack module
    public BulletAttackModule bulletAttackModule;
    public LightningAttackModule lightningAttackModule;
    public RayAttackModule rayAttackModule;

    // floating cooldown between attacks
    public float idleMin;
    public float idleMax;
    
    
    [SerializeField] private iState currentState;
    private Dictionary<BossStateType, iState> _states;

    private void Start()
    {
        if (_states == null)
        {
            _states = new Dictionary<BossStateType, iState>();
            _states.Add(BossStateType.Idle, new Idle(this));
            _states.Add(BossStateType.Attack, new Attack(this));
        }
        TransitionToState(BossStateType.Idle);
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionToState(BossStateType state)
    {
        currentState?.OnExit();
        iState newState = _states[state];
        newState.OnEnter();
        Debug.Log("Transition from " + currentState + " to " + newState);
        currentState = newState;
    }
}
  