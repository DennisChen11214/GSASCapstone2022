using System;
using System.Collections;
using System.Collections.Generic;
using Core.GlobalVariables;
using Core.GlobalEvents;
using Unity.VisualScripting;
using UnityEngine;


public enum BossStateType
{
    Idle,
    Attack,
    Move
}

public class BossStateFSM : MonoBehaviour
{
    public bool DEBUG = true;
    
    [SerializeField] TransformVariable[] _targets;
    private int _targetIdx = 0;
    [SerializeField] GlobalEvent _swap;
    [NonSerialized] public Transform target; // The true target

    public Transform bossPart; // The Actual boss that takes damage and attacks
    

    #region Attack State
    // modules attack module
    public BulletAttackModule bulletAttackModule;
    public LightningAttackModule lightningAttackModule;
    public RayAttackModule rayAttackModule;

    #endregion
    
    
    #region Idle State
    // floating cooldown between attacks (Idle State)
    public float idleMin;
    public float idleMax;

    #endregion


    #region Move State
    public Transform[] locations;
    public float moveCoolDown;
    public bool canMove;
    
    #endregion

    [SerializeField] private iState currentState;
    private Dictionary<BossStateType, iState> _states;

    private void Start()
    {
        target = _targets[_targetIdx].Value;
        _swap.Subscribe(SwapTarget);
        if (_states == null)
        {
            _states = new Dictionary<BossStateType, iState>();
            _states.Add(BossStateType.Idle, new Idle(this));
            _states.Add(BossStateType.Attack, new Attack(this));
            _states.Add(BossStateType.Move, new Move(this));
        }

        StartCoroutine(InitialMoveCoolDown());
        TransitionToState(BossStateType.Idle, "Initialization");
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionToState(BossStateType state, string caller = "")
    {
        if (DEBUG) Debug.Log(caller + " Requests Transition -> " + state + "VVVVVVVVVVVVVVVVVVVV");
        currentState?.OnExit();
        iState newState = _states[state];
        newState.OnEnter();
        if (DEBUG) Debug.Log("Done Transition from " + currentState + " to " + newState + "(" + caller + ")" + "^^^^^^^^^^^^^^^^^^^^");
        currentState = newState;
    }
    
    
    private void SwapTarget()
    {
        _targetIdx = (_targetIdx + 1) % 2;
        target = _targets[_targetIdx].Value;
    }
    
    IEnumerator InitialMoveCoolDown()
    {
        canMove = false;
        yield return new WaitForSeconds(moveCoolDown);
        canMove = true;
    }

    private void OnDestroy()
    {
        _swap.Unsubscribe(SwapTarget);
    }
}