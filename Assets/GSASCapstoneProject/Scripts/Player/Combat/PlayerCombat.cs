///
/// Created by Dennis Chen
///

using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public abstract class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    protected IntVariable _playerHealth;
    [SerializeField]
    protected IntVariable _numDeaths;
    [SerializeField]
    protected FloatVariable _damageDealt;
    [SerializeField]
    protected FloatVariable _damageTaken;
    [SerializeField]
    protected BoolVariable _isKnockedBack;
    [SerializeField]
    protected BoolVariable _isInvincible;
    [SerializeField]
    protected BoolVariable _isPlayerDead;
    [SerializeField]
    protected BoolVariable _isAttacking;
    [SerializeField]
    protected GlobalEvent _onPlayerDied;
    [SerializeField]
    protected GlobalEvent _onPlayerRevived;
    [SerializeField]
    protected GlobalEvent _onOtherPlayerRevived;
    [SerializeField]
    protected ScriptableStats _stats;

    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected float _knockbackTime;
    protected bool _hitFromLeft;

    public abstract void Attack();
    public virtual void VerticalAttack(bool up) { }
    public virtual void CancelAttack() { }
    public virtual void IncreaseCharge(float percent) { }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();        
        _anim = GetComponent<Animator>();
        _onPlayerRevived.Subscribe(Revive);
        _onOtherPlayerRevived.Subscribe(ReviveOtherPlayer);
        ResetValues();
    }

    protected virtual void ResetValues()
    {
        _playerHealth.Value = _playerHealth.DefaultValue;
        _damageDealt.Value = 0;
        _damageTaken.Value = 0;
        _numDeaths.Value = 0;
        _isKnockedBack.Value = false;
        _isInvincible.Value = false;
        _isAttacking.Value = false;
        _isPlayerDead.Value = false;
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate()
    {
        HandleKnockBack();
    }

    public void TakeDamage(bool left)
    {
        if (_isInvincible.Value) return;
        _playerHealth.Value -= 1;
        _damageTaken.Value++;
        if (_playerHealth.Value == 0)
        {
            Die();
            return;
        }
        if (!_isKnockedBack.Value)
        {
            _knockbackTime = _stats.KnockbackLength;
            _isInvincible.Value = true;
            _hitFromLeft = left;
        }
        _isKnockedBack.Value = true;
    }

    private void Die()
    {
        _isPlayerDead.Value = true;
        _numDeaths.Value++;
        _onPlayerDied.Raise();
        gameObject.SetActive(false);
    }

    private void Revive()
    {
        _playerHealth.Value = 1;
    }

    private void ReviveOtherPlayer()
    {
        if(_playerHealth.Value != 1)
        {
            _playerHealth.Value -= 1;
        }
    }

    public void TakeDamageNoKnockback()
    {
        _playerHealth.Value -= 1;
        _damageTaken.Value++;
        if (_playerHealth.Value == 0)
        {
            Die();
            return;
        }
    }

    protected virtual void HandleKnockBack()
    {
        if (_knockbackTime > 0)
        {
            float direction = _hitFromLeft ? -1 : 1;
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

    private void OnDestroy()
    {
        _onPlayerRevived.Unsubscribe(Revive);
        _onOtherPlayerRevived.Unsubscribe(ReviveOtherPlayer);
    }
}
