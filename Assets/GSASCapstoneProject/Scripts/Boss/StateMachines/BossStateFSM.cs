///
/// Created by Dennis Chen + Zhanbo Lin
///

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Transform BossTransform;
    public Transform bossAttackPart; // The Actual boss that takes damage and attacks
    public Animator animator;
    public BossStats Stats;
    public bool DEBUG = true;

    [NonSerialized] public Transform Target; // The true target
    [NonSerialized] public Dictionary<BossAttacks.BossAttack, float> _attackWeightDict;
    [NonSerialized] public Dictionary<BossAttacks.BossAttack, float> _attackDelayDict;
    [NonSerialized ]public float MaxHealth;
    
    [SerializeField] private FloatVariable _health;
    
    [SerializeField] private FloatVariable _enrageHealthThreshold;
    [SerializeField] private BoolVariable _isBossEnraged;
    [SerializeField] private TransformVariable[] _targets;
    [SerializeField] private GlobalEvent _swap;
    [SerializeField] private List<BossAttacks> _bossAttackCombos;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Sprite _enragedSprite;

    private int _targetIdx = 0;
    private bool _enraged = false;

    #region Attack State
    // modules attack module
    public BulletAttackModule BulletAttackModule;
    public WallAttackModule WallAttackModule;
    public ScratchAttackModule ScratchAttackModule;
    [BoolAttribute("Heaven", true, "Stats")]
    public RayAttackModule RayAttackModule;

    #endregion
    
    #region Idle State
    // floating cooldown between attacks (Idle State)
    public float idleMin;
    public float idleMax;

    #endregion


    #region Move State
    public Transform[] locations;
    public float MoveCooldown;
    public bool canMove;
    public float moveSpeed;
    
    #endregion

    [SerializeField] private iState _currentState;
    private Dictionary<BossStateType, iState> _states;

    public BossStateType DebugState { get; private set; }

    private void Start()
    {
        //Choose the player to target
        Target = _targets[_targetIdx].Value;
        _swap.Subscribe(SwapTarget);
        InitializeWeightDict();
        InitializeDelayDict();
        //Sort the predefined boss combos by the health threshold they occur at
        _bossAttackCombos.Sort((a, b) => b.HealthThreshold.CompareTo(a.HealthThreshold));
        for (int i = 0; i < _bossAttackCombos.Count; i++)
        {
            _bossAttackCombos[i].done = false;
        }
        _health.Value = _health.DefaultValue;
        MaxHealth = _health.Value;
        _health.Subscribe(CheckForHealthThreshold);
        //Initialize the list of possible boss states
        if (_states == null)
        {
            _states = new Dictionary<BossStateType, iState>();
            _states.Add(BossStateType.Idle, new Idle(this));
            _states.Add(BossStateType.Attack, new Attack(this));
            _states.Add(BossStateType.Move, new Move(this));
        }

        StartCoroutine(StartMoveCooldown());
        TransitionToState(BossStateType.Idle, "Initialization");
    }

    //Each time the boss loses health, check if any of the health thresholds for the combos have been reached
    //Also checks if the enrage threshold has been reached
    private void CheckForHealthThreshold(float health)
    {
        if(!_enraged && health < MaxHealth * _enrageHealthThreshold.Value)
        {
            _isBossEnraged.Value = true;
            _enraged = true;
            _sprite.sprite = _enragedSprite;
        }
        for (int i = 0; i < _bossAttackCombos.Count; i++)
        {
            if(!_bossAttackCombos[i].done && health <= _bossAttackCombos[i].HealthThreshold * MaxHealth)
            {
                HealthThresholdAttack(_bossAttackCombos[i].AttackCombo);
                _bossAttackCombos[i].done = true;
            }
        }
    }

    //When a health threshold is reached for an attack combo, register it with the attack state
    private void HealthThresholdAttack(List<BossAttacks.BossAttack> attackCombo)
    {
        Debug.Log("Health Threshold Reached");
        ((Attack)_states[BossStateType.Attack]).RegisterAttacks(attackCombo);
        if(_currentState != _states[BossStateType.Attack])
        {
            TransitionToState(BossStateType.Attack, "Health Threshold");
        }
    }

    private void Update()
    {
        _currentState.OnUpdate(Time.deltaTime);
    }

    //Initialize a dictionary of the bosses attacks with the chance of each attack occuring
    private void InitializeWeightDict()
    {
        _attackWeightDict = new Dictionary<BossAttacks.BossAttack, float>();

        _attackWeightDict.Add(BossAttacks.BossAttack.Shotgun, Stats.ShotgunWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Rifle, Stats.RifleWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.ThinWall, Stats.ThinWallWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.ThickWall, Stats.ThickWallWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Lasers, Stats.LaserWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Feathers, Stats.FeatherWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Swipe, Stats.SwipeWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Crush, Stats.CrushWeight);
        _attackWeightDict.Add(BossAttacks.BossAttack.Meteors, Stats.MeteorWeight);
        
        float weightSum = 0;
        foreach ( var p in _attackWeightDict)
        {
            weightSum += p.Value;
        }
        foreach ( var p in _attackWeightDict.ToArray())
        {
            _attackWeightDict[p.Key] /= weightSum;
        }
            
    }

    //Initialize a dictionary of the bosses attacks with the delay of each attack
    private void InitializeDelayDict()
    {
        _attackDelayDict = new Dictionary<BossAttacks.BossAttack, float>();
        _attackDelayDict.Add(BossAttacks.BossAttack.Shotgun, Stats.ShotgunDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Rifle, Stats.RifleDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.ThinWall, Stats.ThinWallDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.ThickWall, Stats.ThickWallDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Lasers, Stats.LaserDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Feathers, Stats.FeatherDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Swipe, Stats.SwipeDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Crush, Stats.CrushDelay);
        _attackDelayDict.Add(BossAttacks.BossAttack.Meteors, Stats.MeteorDelay);
    }

    //Transition from one state to another and potentially print out debug information
    public void TransitionToState(BossStateType state, string caller = "")
    {
        if (DEBUG)
        {
            string boss = Stats.Heaven ? "Heaven: " : "Hell: ";
            Debug.Log(boss + caller + " Requests Transition -> " + state + "VVVVVVVVVVVVVVVVVVVV");
        }
        _currentState?.OnExit();
        iState newState = _states[state];
        newState.OnEnter();
        if (DEBUG) Debug.Log("Done Transition from " + _currentState + " to " + newState + "(" + caller + ")" + "^^^^^^^^^^^^^^^^^^^^");
        _currentState = newState;
        DebugState = state;
    }
    
    //Swap targets to the other player if the two players swap positions
    private void SwapTarget()
    {
        _targetIdx = (_targetIdx + 1) % 2;
        Target = _targets[_targetIdx].Value;
    }
    
    public IEnumerator StartMoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(MoveCooldown);
        canMove = true;
    }

    private void OnDestroy()
    {
        _swap.Unsubscribe(SwapTarget);
        _health.Unsubscribe(CheckForHealthThreshold);
    }
}