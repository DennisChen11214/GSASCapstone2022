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
    [SerializeField]
    protected AudioClip _attackSound;
    [SerializeField]
    protected AudioClip _onHitSound;

    protected AudioSource _audio;
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
        _audio = transform.parent.GetComponent<AudioSource>();
        _onPlayerRevived.Subscribe(Revive);
        _onOtherPlayerRevived.Subscribe(ReviveOtherPlayer);
        ResetValues();
    }

    //Reset player values each time the scene is reloaded
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

    //The player takes damage and gets knocked back
    public void TakeDamage(bool left)
    {
        if (_isInvincible.Value) return;
        _playerHealth.Value -= 1;
        _damageTaken.Value++;
        _audio.PlayOneShot(_onHitSound);
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

    //A player revives with one health;
    private void Revive()
    {
        _playerHealth.Value = 1;
    }

    //Reviving the other player takes one health from this player
    private void ReviveOtherPlayer()
    {
        if(_playerHealth.Value != 1)
        {
            _playerHealth.Value -= 1;
        }
    }

    //The player takes damage without getting knocked back
    public void TakeDamageNoKnockback()
    {
        _playerHealth.Value -= 1;
        _damageTaken.Value++;
        _audio.PlayOneShot(_onHitSound);
        if (_playerHealth.Value == 0)
        {
            Die();
            return;
        }
    }

    //Applies a knockback to the player by changing its velocity
    protected virtual void HandleKnockBack()
    {
        if (_knockbackTime > 0)
        {
            float direction = _hitFromLeft ? -1 : 1;
            //The velocity is lerped from a start and end velocity to feel smoother
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
