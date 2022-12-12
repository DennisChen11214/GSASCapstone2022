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
        _attackEndTime = 0;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //Handles the normal attacks that the player makes
    public override void Attack()
    {
        //If the player just finished a combo and not enough time has passed
        if (_isComboFinished && Time.time - _attackEndTime < _stats.TimeBeforeNextCombo)
        {
            return;
        }
        //Buffer the attack if one is already going on
        if (_isAttacking.Value)
        {
            _hasAttackBuffered = true;
            _hasUpAttackBuffered = false;
            _hasDownAttackBuffered = false;
            return;
        }
        _isComboFinished = false;
        //If enough time has passed since the previous attack, the attack combo resets back to the first attack
        if (Time.time - _attackEndTime > _stats.TimeBeforeAttackResets)
        {
            _currentAttack = 0;
        }
        _isAttacking.Value = true;
        _audio.PlayOneShot(_attackSound);
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

    //Handles the up and down attacks that the player makes
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
            _audio.PlayOneShot(_attackSound);
            _anim.SetTrigger("UpAttack");
        }
        else
        {
            _audio.PlayOneShot(_attackSound);
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

    //Checks if there's any attacks buffered at the end of an attack
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
                    _audio.PlayOneShot(_attackSound);
                    _anim.SetTrigger("Attack2");
                    break;
                case 2:
                    _audio.PlayOneShot(_attackSound);
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
                _audio.PlayOneShot(_attackSound);
                _anim.SetTrigger("DownAttack");
            }
        }
    }

    //Called during the middle of an attack animation to check if the boss in the hit collider
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
        //Deal damage to the boss if its in the hit collider
        foreach (Collider2D enemy in enemiesHit)
        {
            DamageModule damageModule = enemy.GetComponent<DamageModule>();
            if (damageModule != null)
            {
                damageModule.TakeDamage(damage);
                _damageDealt.Value += damage;
                //Have the player bounce up a bit if they hit the boss while down attacking in mid-air
                if (_isDownAttacking && _rb.velocity.y != 0)
                {
                    _onDownAttackInAir.Raise();
                }
                //If the player hits a boss, they float for a bit
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
}
