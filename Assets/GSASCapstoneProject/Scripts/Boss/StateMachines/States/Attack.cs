using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attack : iState
{
    // not being used for now, may be used later for modularize boss attacks
    public enum BossAttackType
    {
        // Direct
        Bullet, Lightning, Ray, Scratch,
        // Hazard
        Wall, Zone, Spiral,
        // Misc
        WalkingBomb,
    }
    
    
    private BossStateFSM _manager;
    private bool doneAttacking;

    
    public Attack(BossStateFSM manager)
    {
        this._manager = manager;
    }
    
    
    public void OnEnter()
    {
        doneAttacking = false;
        _manager.bulletAttackModule.SetTarget(_manager.target.Value);
        _manager.bulletAttackModule.SetBossPart(_manager.transform);
        _manager.bulletAttackModule.Burst();
    }

    public void OnUpdate()
    {
        if (_manager.bulletAttackModule.doneAttacking)
        {
            doneAttacking = true;
        }

        if (doneAttacking)
        {
            _manager.TransitionToState(BossStateType.Idle);
        }
    }

    public void OnExit()
    {
        
    }
    
}
