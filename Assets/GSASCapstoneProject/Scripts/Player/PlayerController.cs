/*// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController {
    [SerializeField] private ScriptableStats _stats;

    #region Internal

    private Rigidbody2D _rb;
    private PlayerInput _input;
    private CapsuleCollider2D[] _cols; // Standing and Crouching colliders
    private CapsuleCollider2D _col; // Current collider
    private Bounds _standingColliderBounds = new(new(0, 0.75f), Vector3.one); // gets overwritten in Awake. When not in play mode, is used for Gizmos
    private bool _cachedTriggerSetting;

    private FrameInput _frameInput;
    private Vector2 _speed;
    private Vector2 _currentExternalVelocity;
    private int _fixedFrame;
    private bool _grounded;
    private bool _isOnWall;
    private bool _crouching;
    private bool _onLadder;

    #endregion

    #region External

    public event Action<bool, float> GroundedChanged;
    public event Action<bool> WallGrabChanged;
    public event Action<bool, Vector2> DashingChanged;
    public event Action<bool> Jumped;
    public event Action DoubleJumped;
    public event Action Attacked;
    public ScriptableStats PlayerStats => _stats;
    public Vector2 Input => _frameInput.Move;
    public Vector2 Speed => _speed;
    public Vector2 GroundNormal => _groundNormal;
    public int WallDirection => _wallDir;
    public bool Crouching => _crouching;
    public bool ClimbingLadder => _onLadder;

    public virtual void ApplyVelocity(Vector2 vel, PlayerForce forceType) {
        if (forceType == PlayerForce.Burst) _speed += vel;
        else _currentExternalVelocity += vel;
    }

    #endregion

    protected virtual void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _cols = GetComponents<CapsuleCollider2D>();

        // Colliders cannot be check whilst disabled. Let's cache its bounds
        _standingColliderBounds = _cols[0].bounds;
        _standingColliderBounds.center = _cols[0].offset;

        Physics2D.queriesStartInColliders = false;
        _cachedTriggerSetting = Physics2D.queriesHitTriggers;

        SetCrouching(false);
    }

    protected virtual void Update() {
        GatherInput();
    }

    protected virtual void GatherInput() {
        _frameInput = _input.FrameInput;

        if (_frameInput.JumpDown) {
            _jumpToConsume = true;
            _frameJumpWasPressed = _fixedFrame;
        }

        if (_frameInput.DashDown && _stats.AllowDash) _dashToConsume = true;
        if (_frameInput.AttackDown) _attackToConsume = true;
    }

    protected virtual void FixedUpdate() {
        _fixedFrame++;
        _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);

        CheckCollisions();

        HandleCollisions();
        HandleLadders();
        HandleAttacking();
        HandleCrouching();
        HandleHorizontal();
        HandleWalls();
        HandleJump();
        HandleDash();
        HandleFall();

        ApplyVelocity();
    }

    #region Collisions

    private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
    private readonly RaycastHit2D[] _ceilingHits = new RaycastHit2D[2];
    private readonly Collider2D[] _wallHits = new Collider2D[5];
    private readonly Collider2D[] _ladderHits = new Collider2D[1];
    private int _groundHitCount;
    private int _ceilingHitCount;
    private int _wallHitCount;
    private int _ladderHitCount;
    private int _frameLeftGrounded = int.MinValue;

    protected virtual void CheckCollisions() {
        Physics2D.queriesHitTriggers = false;
        // Ground and Ceiling
        var origin = (Vector2)transform.position + _col.offset;
        _groundHitCount = Physics2D.CapsuleCastNonAlloc(origin, _col.size, _col.direction, 0, Vector2.down, _groundHits, _stats.GrounderDistance, ~_stats.PlayerLayer);
        _ceilingHitCount = Physics2D.CapsuleCastNonAlloc(origin, _col.size, _col.direction, 0, Vector2.up, _ceilingHits, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Walls and Ladders
        var bounds = GetWallDetectionBounds();
        _wallHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _wallHits, _stats.ClimbableLayer);

        Physics2D.queriesHitTriggers = true; // Ladders are set to Trigger
        _ladderHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _ladderHits, _stats.LadderLayer);

        Physics2D.queriesHitTriggers = _cachedTriggerSetting;
    }

    private Bounds GetWallDetectionBounds() {
        var colliderOrigin = transform.position + _standingColliderBounds.center;
        return new Bounds(colliderOrigin, _stats.WallDetectorSize);
    }

    protected virtual void HandleCollisions() {
        // Hit a Ceiling
        if (_speed.y > 0 && _ceilingHitCount > 0) _speed.y = 0;

        // Landed on the Ground
        if (!_grounded && _groundHitCount > 0) {
            _grounded = true;
            _canDash = true;
            ResetJump();
            GroundedChanged?.Invoke(true, Mathf.Abs(_speed.y));
        }
        // Left the Ground
        else if (_grounded && _groundHitCount == 0) {
            _grounded = false;
            _frameLeftGrounded = _fixedFrame;
            GroundedChanged?.Invoke(false, 0);
        }
    }

    #endregion

    #region Ladders

    private Vector2 _ladderSnapVel;
    private int _frameLeftLadder = int.MinValue;
    private bool LadderInputReached => Mathf.Abs(_frameInput.Move.y) > _stats.LadderClimbThreshold;

    protected virtual void HandleLadders() {
        if (!_onLadder && _ladderHitCount > 0 && LadderInputReached && _fixedFrame > _frameLeftLadder + _stats.LadderCooldownFrames) ToggleClimbingLadders(true);
        else if (_onLadder && _ladderHitCount == 0) ToggleClimbingLadders(false);

        // Snap to center of ladder
        if (_onLadder && _frameInput.Move.x == 0 && _stats.SnapToLadders) {
            var pos = _rb.position;
            _rb.position = Vector2.SmoothDamp(pos, new Vector2(_ladderHits[0].transform.position.x, pos.y), ref _ladderSnapVel, _stats.LadderSnapSpeed);
        }
    }

    private void ToggleClimbingLadders(bool on) {
        if (on) {
            _onLadder = true;
            _speed = Vector2.zero;
        }
        else {
            if (!_onLadder) return;
            _frameLeftLadder = _fixedFrame;
            _onLadder = false;
        }
    }

    #endregion

    #region Attacking

    private bool _attackToConsume;
    private int _frameLastAttacked = int.MinValue;

    protected virtual void HandleAttacking() {
        if (!_attackToConsume) return;

        if (_fixedFrame > _frameLastAttacked + _stats.AttackFrameCooldown) {
            _frameLastAttacked = _fixedFrame;
            Attacked?.Invoke();
        }

        _attackToConsume = false;
    }

    #endregion

    #region Crouching

    private readonly Collider2D[] _crouchHits = new Collider2D[5];
    private int _frameStartedCrouching;

    protected virtual void HandleCrouching() {
        var crouchCheck = _frameInput.Move.y <= _stats.CrouchInputThreshold;
        if (crouchCheck != _crouching) SetCrouching(crouchCheck);
    }

    protected virtual void SetCrouching(bool active) {
        // Prevent standing into colliders
        if (_crouching && !CanStandUp()) return;

        _crouching = active;
        _col = _cols[active ? 1 : 0];
        _cols[0].enabled = !active;
        _cols[1].enabled = active;

        if (_crouching) _frameStartedCrouching = _fixedFrame;
    }

    protected bool CanStandUp() {
        var pos = transform.position + _standingColliderBounds.center;
        pos.y += _standingColliderBounds.extents.y;
        var size = new Vector2(_standingColliderBounds.size.x, _stats.CrouchBufferCheck);
        var hits = Physics2D.OverlapBoxNonAlloc(pos, size, 0, _crouchHits, ~_stats.PlayerLayer);

        return hits == 0;
    }

    #endregion

    #region Horizontal

    protected virtual void HandleHorizontal() {
        if (_frameInput.Move.x != 0) {
            if (_crouching && _grounded) {
                var crouchPoint = Mathf.InverseLerp(0, _stats.CrouchSlowdownFrames, _fixedFrame - _frameStartedCrouching);
                var diminishedMaxSpeed = _stats.MaxSpeed * Mathf.Lerp(1, _stats.CrouchSpeedPenalty, crouchPoint);

                _speed.x = Mathf.MoveTowards(_speed.x, diminishedMaxSpeed * _frameInput.Move.x, _stats.GroundDeceleration * Time.fixedDeltaTime);
            }
            else {
                // Prevent useless horizontal speed buildup when against a wall
                if (Mathf.Approximately(_rb.velocity.x, 0) && Mathf.Sign(_frameInput.Move.x) == Mathf.Sign(_speed.x))
                    _speed.x = 0;

                var inputX = _frameInput.Move.x * _currentWallJumpMoveMultiplier * (_onLadder ? _stats.LadderShimmySpeedMultiplier : 1);
                _speed.x = Mathf.MoveTowards(_speed.x, inputX * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }
        else
            _speed.x = Mathf.MoveTowards(_speed.x, 0, (_grounded ? _stats.GroundDeceleration : _stats.AirDeceleration) * Time.fixedDeltaTime);
    }

    #endregion

    #region Walls

    private float _currentWallJumpMoveMultiplier = 1f;
    private int _wallDir;

    protected virtual void HandleWalls() {
        if (!_stats.AllowWalls) return;

        _currentWallJumpMoveMultiplier = Mathf.MoveTowards(_currentWallJumpMoveMultiplier, 1f, 1f / _stats.WallJumpInputLossFrames);

        // May need to prioritize the nearest wall here... But who is going to make a climbable wall that tight?
        _wallDir = _wallHitCount > 0 ? (int)Mathf.Sign(_wallHits[0].transform.position.x - transform.position.x) : 0;

        if (_isOnWall && !ShouldStickToWall()) SetOnWall(false);
        else if (!_isOnWall && ShouldStickToWall()) SetOnWall(true);

        bool ShouldStickToWall() {
            if (_wallDir == 0) return false;
            if (_stats.RequireInputPush) return Mathf.Sign(_frameInput.Move.x) == _wallDir;
            return true;
        }
    }

    private void SetOnWall(bool on) {
        _isOnWall = on;
        if (!on) _endedJumpEarly = false; // Not flicking this affects future wall jumps
        else _speed = Vector2.zero;
        WallGrabChanged?.Invoke(on);
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
    private bool HasBufferedJump => (_grounded || _isOnWall) && _bufferedJumpUsable && _fixedFrame < _frameJumpWasPressed + _stats.JumpBufferFrames;
    private bool CanDoubleJump => _stats.AllowDoubleJump && _doubleJumpUsable && !_coyoteUsable;

    protected virtual void HandleJump() {
        // Wall jump
        if (!_grounded && ((_isOnWall && _jumpToConsume) || HasBufferedJump)) {
            var power = _stats.WallJumpPower;
            power.x *= -_wallDir;
            _speed = power;
            _jumpToConsume = false;
            if (_stats.AllowWalls) _currentWallJumpMoveMultiplier = 0;
            _bufferedJumpUsable = false;
            SetOnWall(false);
            Jumped?.Invoke(true);
        }

        // Double jump
        if (_jumpToConsume && CanDoubleJump && !_onLadder) {
            _speed.y = _stats.JumpPower;
            _doubleJumpUsable = false;
            _endedJumpEarly = false;
            _jumpToConsume = false;
            DoubleJumped?.Invoke();
        }

        // Standard jump
        if ((_jumpToConsume && (CanUseCoyote || _onLadder)) || HasBufferedJump) {
            _coyoteUsable = false;
            _bufferedJumpUsable = false;
            _speed.y = _stats.JumpPower;
            ToggleClimbingLadders(false);
            Jumped?.Invoke(false);
        }

        // Early end detection
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
    }

    protected virtual void ResetJump() {
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

    protected virtual void HandleDash() {
        if (_dashToConsume && _canDash && !_crouching) {
            var dir = new Vector2(_frameInput.Move.x, Mathf.Max(_frameInput.Move.y, 0f)).normalized;
            if (dir == Vector2.zero) {
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

        if (_dashing) {
            _speed = _dashVel;
            // Cancel when the time is out or we've reached our max safety distance
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames) {
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

    protected virtual void HandleFall() {
        if (_dashing) return;

        // Ladder
        if (_onLadder) {
            var multiplier = _frameInput.Move.y < 0 ? _stats.LadderSlideMultiplier : 1;
            _speed.y = _frameInput.Move.y * _stats.LadderClimbSpeed * multiplier;

            return;
        }

        // Grounded & Slopes
        if (_grounded && _speed.y <= 0f) {
            _speed.y = _stats.GroundingForce;

            // We use a raycast here as the groundHits from capsule cast act a bit weird.
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _stats.GrounderDistance * 2, ~_stats.PlayerLayer);
            if (hit.collider != null) {
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

        // Wall Climbing & Sliding
        if (_isOnWall) {
            _speed.y = Mathf.MoveTowards(_speed.y, -_stats.MaxWallFallSpeed, _stats.WallFallAcceleration * Time.fixedDeltaTime);

            if (_frameInput.Move.y > 0)
                _speed.y = _stats.WallClimbSpeed;

            if (_rb.velocity.y < 0) {
                _speed.y = Mathf.MoveTowards(_speed.y, -_stats.MaxWallFallSpeed, _stats.WallFallAcceleration * Time.fixedDeltaTime);
                return;
            }
        }

        // In Air
        var fallSpeed = _stats.FallAcceleration;
        if (_endedJumpEarly && _speed.y > 0) fallSpeed *= _stats.JumpEndEarlyGravityModifier * _currentWallJumpMoveMultiplier;
        _speed.y = Mathf.MoveTowards(_speed.y, -_stats.MaxFallSpeed, fallSpeed * Time.fixedDeltaTime);
    }

    #endregion

    protected virtual void ApplyVelocity() {
        _rb.velocity = _speed + _currentExternalVelocity;
        _jumpToConsume = false;
    }

    private void OnDrawGizmos() {
        if (_stats.ShowWallDetection) {
            Gizmos.color = Color.white;
            var bounds = GetWallDetectionBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}

public interface IPlayerController {
    /// <summary>
    /// true = Landed. false = Left the Ground. float is Impact Speed
    /// </summary>
    public event Action<bool, float> GroundedChanged;

    public event Action<bool> WallGrabChanged;
    public event Action<bool, Vector2> DashingChanged; // Dashing - Dir
    public event Action<bool> Jumped; // Is wall jump
    public event Action DoubleJumped;
    public event Action Attacked;
    public ScriptableStats PlayerStats { get; }
    public Vector2 Input { get; }
    public Vector2 Speed { get; }
    public Vector2 GroundNormal { get; }
    public int WallDirection { get; }
    public bool Crouching { get; }
    public bool ClimbingLadder { get; }
    public void ApplyVelocity(Vector2 vel, PlayerForce forceType);
}

public enum PlayerForce {
    /// <summary>
    /// Added directly to the players movement speed, to be controlled by the standard deceleration
    /// </summary>
    Burst,

    /// <summary>
    /// An additive force handled by the decay system
    /// </summary>
    Decay
}
*/