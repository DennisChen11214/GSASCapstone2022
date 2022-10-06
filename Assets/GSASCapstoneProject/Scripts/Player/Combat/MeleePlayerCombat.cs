using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Core.GlobalVariables;

public class MeleePlayerCombat : PlayerCombat
{
    [SerializeField]
    Transform _attackPos;
    [SerializeField]
    LayerMask _enemyLayer;
    [SerializeField]
    float _attackRadius;
    [SerializeField]
    float _attackPower;

    private float _attackEndTime;
    private bool _hasAttackBuffered = false;
    private bool _isAttacking = false;
    private int _currentAttack = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        if (_isAttacking)
        {
            _hasAttackBuffered = true;
            return;
        }
        if(Time.time - _attackEndTime > _stats.TimeBeforeAttackResets)
        {
            _currentAttack = 0;
        }
        _isAttacking = true;
        switch (_currentAttack)
        {
            case 0:
                _anim.SetTrigger("Attack1");
                break;
            case 1:
                _anim.SetTrigger("Attack2");
                break;
            case 2:
                _anim.SetTrigger("Attack3");
                break;
        }
    }

    public void OnAttackEnd()
    {
        _attackEndTime = Time.time;
        _isAttacking = false;
        _currentAttack = (_currentAttack + 1) % 3;
        CheckForBufferedAttack();
    }

    private void CheckForBufferedAttack()
    {
        if (_hasAttackBuffered)
        {
            _isAttacking = true;
            _hasAttackBuffered = false;
            switch (_currentAttack)
            {
                case 0:
                    _isAttacking = false;
                    break;
                case 1:
                    _anim.SetTrigger("Attack2");
                    break;
                case 2:
                    _anim.SetTrigger("Attack3");
                    break;
            }
        }
    }

    public void CheckHit()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(_attackPos.position, _attackRadius, _enemyLayer);
        foreach (Collider2D enemy in enemiesHit)
        {
            DamageModule damageModule = enemy.GetComponent<DamageModule>();
            if (damageModule != null)
            {
                damageModule.TakeDamage(_attackPower);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_attackPos.position, _attackRadius);
    }

    private void OnEnable()
    {
        _hasAttackBuffered = false;
        _isAttacking = false;
    }
}
