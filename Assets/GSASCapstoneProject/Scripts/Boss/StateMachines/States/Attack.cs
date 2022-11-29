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
    private List<BossAttacks.BossAttack> _attackQueue;

    public Attack(BossStateFSM manager)
    {
        this._manager = manager;
        _attackQueue = new List<BossAttacks.BossAttack>();
    }
    
    
    public void OnEnter()
    {
        if(_attackQueue.Count > 0)
        {
            DoAttack(_attackQueue[0]);
            _attackQueue.RemoveAt(0);
        }
        else
        {
            ChooseRandomAttack();
        }
        if (_manager.DEBUG) Debug.Log("Attack Done OnEnter()");
    }

    private void ChooseRandomAttack()
    {
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);
        foreach(KeyValuePair<BossAttacks.BossAttack, float> attackWeight in _manager._attackWeightDict)
        {
            if(rand <= attackWeight.Value)
            {
                DoAttack(attackWeight.Key);
                return;
            }
            rand -= attackWeight.Value;
        }
    }

    private void DoAttack(BossAttacks.BossAttack attack)
    {
        delay = _manager._attackDelayDict[attack];
        switch (attack)
        {
            case BossAttacks.BossAttack.Shotgun:
                _manager.BulletAttackModule.SetTarget(_manager.Target);
                _manager.BulletAttackModule.SetBossPart(_manager.bossAttackPart);
                _manager.BulletAttackModule.Burst(_manager.Stats.BurstBullets);
                break;
            case BossAttacks.BossAttack.Rifle:
                break;
            case BossAttacks.BossAttack.ThinWall:
                _manager.WallAttackModule.StartAttack(true);
                break;
            case BossAttacks.BossAttack.ThickWall:
                _manager.WallAttackModule.StartAttack(false);
                break;
            case BossAttacks.BossAttack.Lasers:
                int numRays = UnityEngine.Random.Range(_manager.Stats.MinRays, _manager.Stats.MaxRays);
                _manager.RayAttackModule.Attack(_manager.Target, numRays);
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

    public void RegisterAttacks(List<BossAttacks.BossAttack> attacks)
    {
        _attackQueue.Clear();
        for(int i = 0; i < attacks.Count; i++)
        {
            _attackQueue.Add(attacks[i]);
        }
    }

    public void OnUpdate(float dt)
    {
        delay -= dt;
        if(delay <= 0)
        {
            if(_attackQueue.Count == 0)
            {
                _manager.TransitionToState(BossStateType.Idle, "Attack");
            }
            else
            {
                DoAttack(_attackQueue[0]);
                _attackQueue.RemoveAt(0);
            }
        }
    }

    public void OnExit()
    {
        if (_manager.DEBUG) Debug.Log("Attack Done OnExit()");
    }
    
}
