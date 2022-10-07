using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public abstract class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    protected IntVariable _playerHealth;
    [SerializeField]
    protected BoolVariable _isKnockedBack;
    [SerializeField]
    protected ScriptableStats _stats;

    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected float _knockbackTime;

    public abstract void Attack();

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();        
        _anim = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        HandleKnockBack();
    }

    public void TakeDamage(int damage)
    {
        _playerHealth.Value -= damage;
        if (!_isKnockedBack.Value)
        {
            _knockbackTime = _stats.KnockbackLength;
        }
        _isKnockedBack.Value = true;
    }

    public void TakeDamage()
    {
        _playerHealth.Value -= 1;
        _knockbackTime = _stats.KnockbackLength;
        _isKnockedBack.Value = true;
    }

    protected virtual void HandleKnockBack()
    {
        if (_knockbackTime > 0)
        {
            float direction = transform.localScale.x > 0 ? 1 : -1;
            Vector2 startVel = new Vector2(-_stats.Knockback * direction, _stats.Knockback);
            Vector2 endVel = new Vector2(-_stats.Knockback * direction * _stats.KnockbackFactor, 0);
            float time = 1 - _knockbackTime / _stats.KnockbackLength;
            _rb.velocity = Vector2.Lerp(startVel, endVel, time);
            _knockbackTime -= Time.deltaTime;
            if(_knockbackTime <= 0)
            {
                _rb.velocity = endVel;
            }
        }
    }
}