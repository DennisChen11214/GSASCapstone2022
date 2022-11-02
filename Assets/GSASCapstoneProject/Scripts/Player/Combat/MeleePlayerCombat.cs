///
/// Created by Dennis Chen
///

using UnityEngine;
using Core.GlobalEvents;
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
    GlobalEvent _onDownAttackInAir;
    [SerializeField]
    FloatVariable _floatTime;
    [SerializeField]
    AnimationClip[] _animClips;

    private float _attackEndTime;
    private bool _hasAttackBuffered = false;
    private bool _hasUpAttackBuffered = false;
    private bool _hasDownAttackBuffered = false;
    private bool _isUpAttacking = false;
    private bool _isDownAttacking = false;
    private bool _isComboFinished = false;
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
        if (_isComboFinished && Time.time - _attackEndTime < _stats.TimeBeforeNextCombo)
        {
            return;
        }
        if (_isAttacking.Value)
        {
            _hasAttackBuffered = true;
            _hasUpAttackBuffered = false;
            _hasDownAttackBuffered = false;
            return;
        }
        _isComboFinished = false;
        if (Time.time - _attackEndTime > _stats.TimeBeforeAttackResets)
        {
            _currentAttack = 0;
        }
        _isAttacking.Value = true;
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

    public override void VerticalAttack(bool up)
    {
        if (_isComboFinished && Time.time - _attackEndTime < _stats.TimeBeforeNextCombo)
        {
            return;
        }
        if (_isAttacking.Value)
        {
            _hasAttackBuffered = false;
            _hasUpAttackBuffered = up;
            _hasDownAttackBuffered = !up;
            return;
        }
        if(_isDownAttacking || _isUpAttacking)
        {
            return;
        }
        _isComboFinished = false;
        _isUpAttacking = up;
        _isDownAttacking = !up;
        if (up)
        {
            _anim.SetTrigger("UpAttack");
        }
        else
        {
            _anim.SetTrigger("DownAttack");
        }
    }

    public void OnAttackEnd()
    {
        _attackEndTime = Time.time;
        _isAttacking.Value = false;
        _currentAttack = (_currentAttack + 1) % 3;
        if (_currentAttack == 0)
        {
            _isComboFinished = true;
        }
        CheckForBufferedAttack();
    }

    public void OnVerticalAttackEnd()
    {
        _attackEndTime = Time.time;
        _isUpAttacking = false;
        _isDownAttacking = false;
        _currentAttack = 0;
        _isComboFinished = true;
    }

    private void CheckForBufferedAttack()
    {
        if (_hasAttackBuffered)
        {
            _isAttacking.Value = true;
            _hasAttackBuffered = false;
            switch (_currentAttack)
            {
                case 0:
                    _isAttacking.Value = false;
                    break;
                case 1:
                    _anim.SetTrigger("Attack2");
                    break;
                case 2:
                    _anim.SetTrigger("Attack3");
                    break;
            }
        }
        else if (_hasUpAttackBuffered)
        {
            _hasUpAttackBuffered = false;
            if (_currentAttack != 0)
            {
                _isUpAttacking = true;
                _anim.SetTrigger("UpAttack");
            }
        }
        else if (_hasDownAttackBuffered)
        {
            _hasDownAttackBuffered = false;
            if (_currentAttack != 0)
            {
                _isDownAttacking = true;
                _anim.SetTrigger("DownAttack");
            }
        }
    }

    public void CheckHit()
    {
        float damage = 0;
        if (_isAttacking.Value)
        {
            switch (_currentAttack)
            {
                case 0:
                    damage = _stats.MeleeDamageAttack1;
                    break;
                case 1:
                    damage = _stats.MeleeDamageAttack2;
                    break;
                case 2:
                    damage = _stats.MeleeDamageAttack3;
                    break;
            }
        }
        else
        {
            damage = _stats.MeleeDamageVerticalAttack;
        }
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(_attackPos.position, _attackRadius, _enemyLayer);
        foreach (Collider2D enemy in enemiesHit)
        {
            DamageModule damageModule = enemy.GetComponent<DamageModule>();
            if (damageModule != null)
            {
                damageModule.TakeDamage(damage);
                if (_isDownAttacking && _rb.velocity.y != 0)
                {
                    _onDownAttackInAir.Raise();
                }
                else
                {
                    _floatTime.Value = _animClips[_currentAttack].length;
                }
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
        _hasDownAttackBuffered = false;
        _hasUpAttackBuffered = false;
        _isAttacking.Value = false;
        _isUpAttacking = false;
        _isDownAttacking = false;
    }

    private void OnDisable()
    {
        
    }
}
