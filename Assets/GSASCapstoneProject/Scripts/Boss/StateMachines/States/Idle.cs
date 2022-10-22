using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Idle : iState
{
    private BossStateFSM _manager;

    private bool _canAct;
    
    private float _floatingCoolDown;
    
    private Coroutine _idleCouroutine = null;
    
        
    public Idle(BossStateFSM manager)
    {
        this._manager = manager;
    }

    public void OnEnter()
    {
        _canAct = false;
        System.Random random = new System.Random();
        _floatingCoolDown = (float) random.NextDouble() * (_manager.idleMax - _manager.idleMin);
        _idleCouroutine = _manager.StartCoroutine(_Idle());
        if (_manager.DEBUG) Debug.Log("Idle Done OnEnter()");
    }

    public void OnUpdate()
    {
        // check for conditions of early exit, such as a player is approaching (not yet implemented)
        if (_canAct)
        {
            if (_manager.canMove)
            {
                _manager.TransitionToState(BossStateType.Move, "Idle");
            }
        }
    }
    
    
    public void OnExit()
    {
        _canAct = false;
        _manager.StopCoroutine(_idleCouroutine);
        if (_manager.DEBUG) Debug.Log("Idle Done OnExit()");
    }

    private IEnumerator _Idle()
    {
        yield return new WaitForSeconds(_manager.idleMin);
        _canAct = true;
        yield return new WaitForSeconds(_floatingCoolDown);
        _manager.TransitionToState(BossStateType.Attack, "Idle");
        yield return null;
    }
}
