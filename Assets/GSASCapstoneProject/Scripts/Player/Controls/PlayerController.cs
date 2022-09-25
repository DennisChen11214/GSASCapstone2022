using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats _stats;

    #region Internal

    private Rigidbody2D _rb;
    private PlayerInputActions _input;
    private PlayerCombat _playerCombat;
    private SpriteRenderer _sprite;
    private CapsuleCollider2D _col; // Current collider
    private Bounds _standingColliderBounds = new(new(0, 0.75f), Vector3.one); // gets overwritten in Awake. When not in play mode, is used for Gizmos
    private bool _cachedTriggerSetting;

    private FrameInput _frameInput;
    private Vector2 _speed;
    private Vector2 _currentExternalVelocity;
    private int _fixedFrame;
    private bool _grounded;

    #endregion

    #region External

    public event Action<bool, float> GroundedChanged;
    public event Action<bool, Vector2> DashingChanged;
    public event Action<bool> Jumped;
    public event Action DoubleJumped;
    public ScriptableStats PlayerStats => _stats;
    public Vector2 Input => _frameInput.Move;
    public Vector2 Speed => _speed;
    public Vector2 GroundNormal => _groundNormal;

    #endregion

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInputActions>();
        _col = GetComponent<CapsuleCollider2D>();
        _playerCombat = GetComponent<PlayerCombat>();
        _sprite = GetComponent<SpriteRenderer>();

        // Colliders cannot be check whilst disabled. Let's cache its bounds
        _standingColliderBounds = _col.bounds;
        _standingColliderBounds.center = _col.offset;

        _cachedTriggerSetting = Physics2D.queriesHitTriggers;

    }

    protected virtual void Update()
    {
        GatherInput();
    }

    protected virtual void GatherInput()
    {
        _frameInput = _input.FrameInput;

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _frameJumpWasPressed = _fixedFrame;
        }

        if (_frameInput.DashDown && _stats.AllowDash) _dashToConsume = true;
        if (_frameInput.AttackDown) _attackToConsume = true;
    }

    protected virtual void FixedUpdate()
    {
        _fixedFrame++;
        _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);

        CheckCollisions();

        HandleCollisions();
        HandleAttacking();
        HandleHorizontal();
        HandleJump();
        HandleDash();
        HandleFall();

        ApplyVelocity();
    }

    #region Collisions

    private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
    private readonly RaycastHit2D[] _ceilingHits = new RaycastHit2D[2];
    private int _groundHitCount;
    private int _ceilingHitCount;
    private int _frameLeftGrounded = int.MinValue;

    protected virtual void CheckCollisions()
    {
        Physics2D.queriesHitTriggers = false;
        // Ground and Ceiling
        var origin = (Vector2)transform.position + _col.offset;
        _groundHitCount = Physics2D.CapsuleCastNonAlloc(origin, _col.size, _col.direction, 0, Vector2.down, _groundHits, _stats.GrounderDistance, ~_stats.PlayerLayer);
        _ceilingHitCount = Physics2D.CapsuleCastNonAlloc(origin, _col.size, _col.direction, 0, Vector2.up, _ceilingHits, _stats.GrounderDistance, ~_stats.PlayerLayer);

        Physics2D.queriesHitTriggers = _cachedTriggerSetting;
    }

    protected virtual void HandleCollisions()
    {
        // Hit a Ceiling
        if (_speed.y > 0 && _ceilingHitCount > 0) _speed.y = 0;

        // Landed on the Ground
        if (!_grounded && _groundHitCount > 0)
        {
            _grounded = true;
            _canDash = true;
            ResetJump();
            GroundedChanged?.Invoke(true, Mathf.Abs(_speed.y));
        }
        // Left the Ground
        else if (_grounded && _groundHitCount == 0)
        {
            _grounded = false;
            _frameLeftGrounded = _fixedFrame;
            GroundedChanged?.Invoke(false, 0);
        }
    }

    #endregion

    #region Attacking

    private bool _attackToConsume;

    protected virtual void HandleAttacking()
    {
        if (!_attackToConsume) return;

        _playerCombat.Attack();
        
        _attackToConsume = false;
    }

    #endregion

    #region Horizontal

    protected virtual void HandleHorizontal()
    {
        if (_frameInput.Move.x != 0)
        {
            var inputX = _frameInput.Move.x;
            _sprite.flipX = inputX == 1 ? false : true;
            _speed.x = Mathf.MoveTowards(_speed.x, inputX * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
        else
            _speed.x = Mathf.MoveTowards(_speed.x, 0, (_grounded ? _stats.GroundDeceleration : _stats.AirDeceleration) * Time.fixedDeltaTime);
    }

    #endregion

    #region Jump

    private bool _jumpToConsume;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private bool _doubleJumpUsable;
    private bool _bufferedJumpUsable;
    private int _frameJumpWasPressed = int.MinValue;

    private bool CanUseCoyote => _coyoteUsable && !_grounded && _fixedFrame < _frameLeftGrounded + _stats.CoyoteFrames;
    private bool HasBufferedJump => _grounded && _bufferedJumpUsable && _fixedFrame < _frameJumpWasPressed + _stats.JumpBufferFrames;
    private bool CanDoubleJump => _stats.AllowDoubleJump && _doubleJumpUsable && !_coyoteUsable;

    protected virtual void HandleJump()
    {
        // Double jump
        if (_jumpToConsume && CanDoubleJump)
        {
            _speed.y = _stats.JumpPower;
            _doubleJumpUsable = false;
            _endedJumpEarly = false;
            _jumpToConsume = false;
            DoubleJumped?.Invoke();
        }

        // Standard jump
        if ((_jumpToConsume && CanUseCoyote) || HasBufferedJump)
        {
            _coyoteUsable = false;
            _bufferedJumpUsable = false;
            _speed.y = _stats.JumpPower;
            Jumped?.Invoke(false);
        }

        // Early end detection
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
    }

    protected virtual void ResetJump()
    {
        _coyoteUsable = true;
        _doubleJumpUsable = true;
        _bufferedJumpUsable = true;
        _endedJumpEarly = false;
    }

    #endregion

    #region Dash

    private bool _dashToConsume;
    private bool _canDash;
    private Vector2 _dashVel;
    private bool _dashing;
    private int _startedDashing;

    protected virtual void HandleDash()
    {
        if (_dashToConsume && _canDash)
        {
            var dir = new Vector2(_frameInput.Move.x, 0f).normalized;
            if (dir == Vector2.zero)
            {
                _dashToConsume = false;
                return;
            }

            _dashVel = dir * _stats.DashVelocity;
            _dashing = true;
            DashingChanged?.Invoke(true, dir);
            _canDash = false;
            _startedDashing = _fixedFrame;

            // Strip external buildup
            _currentExternalVelocity = Vector2.zero;
        }

        if (_dashing)
        {
            _speed = _dashVel;
            // Cancel when the time is out or we've reached our max safety distance
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames)
            {
                _dashing = false;
                DashingChanged?.Invoke(false, Vector2.zero);
                if (_speed.y > 0) _speed.y = 0;
                _speed.x *= _stats.DashEndHorizontalMultiplier;
                if (_grounded) _canDash = true;
            }
        }

        _dashToConsume = false;
    }

    #endregion

    #region Falling

    private Vector2 _groundNormal;

    protected virtual void HandleFall()
    {
        if (_dashing) return;

        // Grounded & Slopes
        if (_grounded && _speed.y <= 0f)
        {
            _speed.y = _stats.GroundingForce;

            // We use a raycast here as the groundHits from capsule cast act a bit weird.
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _col.size.y / 2 + _stats.GrounderDistance * 2, ~_stats.PlayerLayer);
            if (hit.collider != null)
            {
                _groundNormal = hit.normal;

                if (!Mathf.Approximately(_groundNormal.y, 1f)) // on a slope
                {
                    _speed.y = _speed.x * -_groundNormal.x / _groundNormal.y;
                    if (_speed.x != 0) _speed.y += _stats.GroundingForce;
                }
            }
            else
                _groundNormal = Vector2.zero;

            return;
        }

        // In Air
        var fallSpeed = _stats.FallAcceleration;
        if (_endedJumpEarly && _speed.y > 0) fallSpeed *= _stats.JumpEndEarlyGravityModifier;
        _speed.y = Mathf.MoveTowards(_speed.y, -_stats.MaxFallSpeed, fallSpeed * Time.fixedDeltaTime);
    }

    #endregion

    protected virtual void ApplyVelocity()
    {
        _rb.velocity = _speed + _currentExternalVelocity;
        _jumpToConsume = false;
    }
}

public interface IPlayerController
{
    /// <summary>
    /// true = Landed. false = Left the Ground. float is Impact Speed
    /// </summary>
    public event Action<bool, float> GroundedChanged;

    public event Action<bool, Vector2> DashingChanged; // Dashing - Dir
    public event Action<bool> Jumped; // Is wall jump
    public event Action DoubleJumped;
    public ScriptableStats PlayerStats { get; }
    public Vector2 Input { get; }
    public Vector2 Speed { get; }
    public Vector2 GroundNormal { get; }
}