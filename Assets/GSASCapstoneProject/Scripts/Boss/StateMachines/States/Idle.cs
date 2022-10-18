using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Idle : iState
{
    private BossStateFSM _manager;

    private bool _canAttack;
    
    private float _floatingCoolDown;
    
    
    public Idle(BossStateFSM manager)
    {
        this._manager = manager;
    }

    public void OnEnter()
    {
        _canAttack = false;
        System.Random random = new System.Random();
        _floatingCoolDown = (float) random.NextDouble() * (_manager.idleMax - _manager.idleMin);
        _manager.StartCoroutine(CoolDown());
    }

    public void OnUpdate()
    {
        // check for conditions of early exit, such as a player is approaching (not yet implemented)
        if (_canAttack)
        {
            
        }
    }

    public void OnExit()
    {
        _manager.StopCoroutine(CoolDown());
        _canAttack = false;
    }

    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_manager.idleMin);
        _canAttack = true;
        yield return new WaitForSeconds(_floatingCoolDown);
        _manager.TransitionToState(BossStateType.Attack);
    }
}
