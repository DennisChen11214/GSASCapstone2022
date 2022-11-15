using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[Serializable]
public class Attack : iState
{   
    private BossStateFSM _manager;
    private float delay;

    public Attack(BossStateFSM manager)
    {
        this._manager = manager;
    }
    
    
    public void OnEnter()
    {
        ChooseRandomAttack();
        if (_manager.DEBUG) Debug.Log("Attack Done OnEnter()");
    }

    private void ChooseRandomAttack()
    {
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);
        foreach(KeyValuePair<BossAttacks.BossAttack, float> attackWeight in _manager._attackWeightDict)
        {
            if(rand <= attackWeight.Value)
            {
                DoRandomAttack(attackWeight.Key);
                return;
            }
            rand -= attackWeight.Value;
        }
    }

    private void DoRandomAttack(BossAttacks.BossAttack attack)
    {
        if (_manager.DEBUG) Debug.Log(attack.ToString());
        delay = _manager._attackDelayDict[attack];
        switch (attack)
        {
            case BossAttacks.BossAttack.Shotgun:
                _manager.bulletAttackModule.SetTarget(_manager.target);
                _manager.bulletAttackModule.SetBossPart(_manager.bossPart);
                _manager.bulletAttackModule.Burst();
                break;
            case BossAttacks.BossAttack.Rifle:
                break;
            case BossAttacks.BossAttack.ThinWall:
                _manager.wallAttackModule.StartAttack(true);
                break;
            case BossAttacks.BossAttack.ThickWall:
                _manager.wallAttackModule.StartAttack(false);
                break;
            case BossAttacks.BossAttack.Lasers:
                break;
            case BossAttacks.BossAttack.Feathers:
                break;
            case BossAttacks.BossAttack.Swipe:
                break;
            case BossAttacks.BossAttack.Crush:
                break;
            case BossAttacks.BossAttack.Meteors:
                break;

        }
    }

    public void OnUpdate(float dt)
    {
        delay -= dt;
        if(delay <= 0)
        {
            _manager.TransitionToState(BossStateType.Idle, "Attack");
        }
    }

    public void OnExit()
    {
        if (_manager.DEBUG) Debug.Log("Attack Done OnExit()");
    }
    
}
