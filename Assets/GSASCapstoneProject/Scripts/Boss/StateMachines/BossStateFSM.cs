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
    public Transform bossPart; // The Actual boss that takes damage and attacks
    public bool DEBUG = true;

    [NonSerialized] public Transform target; // The true target
    [NonSerialized] public Dictionary<BossAttacks.BossAttack, float> _attackWeightDict;
    [NonSerialized] public Dictionary<BossAttacks.BossAttack, float> _attackDelayDict;
    [SerializeField] BossStats _stats;
    
    [SerializeField] TransformVariable[] _targets;
    [SerializeField] GlobalEvent _swap;

    private int _targetIdx = 0;

    #region Attack State
    // modules attack module
    public BulletAttackModule bulletAttackModule;
    public LightningAttackModule lightningAttackModule;
    public RayAttackModule rayAttackModule;
    public WallAttackModule wallAttackModule;

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
        InitializeWeightDict();
        InitializeDelayDict();
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
        currentState.OnUpdate(Time.deltaTime);
    }

    private void InitializeWeightDict()
    {
        _attackWeightDict = new Dictionary<BossAttacks.BossAttack, float>();
        _attackWeightDict.Add(BossAttacks.BossAttack.Shotgun, _stats.ShotgunWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Rifle, _stats.RifleWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.ThinWall, _stats.ThinWallWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.ThickWall, _stats.ThickWallWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Lasers, _stats.LaserWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Feathers, _stats.FeatherWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Swipe, _stats.SwipeWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Crush, _stats.CrushWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Meteors, _stats.MeteorWeight);
    }

    private void InitializeDelayDict()
    {
        _attackDelayDict = new Dictionary<BossAttacks.BossAttack, float>();
        _attackDelayDict.Add(BossAttacks.BossAttack.Shotgun, _stats.ShotgunDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Rifle, _stats.RifleDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.ThinWall, _stats.ThinWallDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.ThickWall, _stats.ThickWallDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Lasers, _stats.LaserDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Feathers, _stats.FeatherDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Swipe, _stats.SwipeDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Crush, _stats.CrushDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Meteors, _stats.MeteorDelay);
    }

    public void TransitionToState(BossStateType state, string caller = "")
    {
        if (DEBUG)
        {
            string boss = _stats.Heaven ? "Heaven: " : "Hell: ";
            Debug.Log(boss + caller + " Requests Transition -> " + state + "VVVVVVVVVVVVVVVVVVVV");
        }
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