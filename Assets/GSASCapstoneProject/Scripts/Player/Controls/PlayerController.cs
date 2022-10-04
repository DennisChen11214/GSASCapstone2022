using System;
using System.Collections;
using UnityEngine;
using Core.GlobalEvents;
using Core.GlobalVariables;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private TransformGlobalEvent _requestSwap;
    [SerializeField] private TransformGlobalEvent _receiveSwapRequest;
    [SerializeField] private GlobalEvent _swapCompleted;
    [SerializeField] private GlobalEvent _swapCanceled;
    [SerializeField] private TransformVariable _playerTransform;
    [SerializeField] private PhysicsMaterial2D _movingMaterial;
    [SerializeField] private PhysicsMaterial2D _stationaryMaterial;
    [SerializeField] private FloatVariable _swapDelay;

    #region Internal

    private Rigidbody2D _rb;
    private PlayerCombat _playerCombat;
    private SpriteRenderer _sprite;
    private BoxCollider2D _col; // Current collider
    private bool _cachedTriggerSetting;

    private Vector2 _moveDirection;
    private Vector2 _speed;
    private Vector2 _currentExternalVelocity;
    private Transform _otherPlayer;
    private int _fixedFrame;
    private bool _grounded;

    private InputActionAsset _actions;
    private InputAction _move, _jump, _dash, _attack, _swap;

    #endregion

    #region External
    public ScriptableStats PlayerStats => _stats;
    public Vector2 Input => _moveDirection;
    public Vector2 Speed => _speed;
    public Vector2 GroundNormal => _groundNormal;

    #endregion

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        _playerCombat = GetComponent<PlayerCombat>();
        _sprite = GetComponent<SpriteRenderer>();
        _actions = GetComponentInParent<PlayerInput>().actions;

        _playerTransform.Value = transform;
        _cachedTriggerSetting = Physics2D.queriesHitTriggers;

        _move = _actions.FindActionMap("Player").FindAction("Movement");
        _jump = _actions.FindActionMap("Player").FindAction("Jump");
        _dash = _actions.FindActionMap("Player").FindAction("Movement Ability");
        _attack = _actions.FindActionMap("Player").FindAction("Attack");
        _swap = _actions.FindActionMap("Player").FindAction("Swap");

        _attack.performed += ctx => HandleAttacking();
        _jump.started += ctx => HandleJump();
        _jump.canceled += ctx => CancelJump();
        _dash.performed += ctx => StartDash();
        _swap.started += ctx => RequestSwap();
        _swap.canceled += ctx => CancelSwap();
    }

    protected virtual void FixedUpdate()
    {
        _fixedFrame++;
        _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);
        _moveDirection = _move.ReadValue<Vector2>();
        CheckCollisions();

        HandleCollisions();
        HandleHorizontal();
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
        Vector2 origin = (Vector2)transform.position + _col.offset * transform.localScale;
        Vector2 _absScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        _groundHitCount = Physics2D.BoxCastNonAlloc(origin, _col.size * _absScale, 0, Vector2.down, _groundHits, _stats.GrounderDistance, ~_stats.PlayerLayer);
        _ceilingHitCount = Physics2D.BoxCastNonAlloc(origin, _col.size * _absScale, 0, Vector2.up, _ceilingHits, _stats.GrounderDistance, ~_stats.PlayerLayer);

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
        }
        // Left the Ground
        else if (_grounded && _groundHitCount == 0)
        {
            _grounded = false;
            _frameLeftGrounded = _fixedFrame;
        }
    }

    #endregion

    #region Swapping
    private bool _swapHeld;

    protected virtual void RequestSwap()
    {
        _swapHeld = true;
        _requestSwap.Raise(transform);
    }

    protected virtual void CancelSwap()
    {
        _swapHeld = false;
        _swapCanceled.Raise();
    }

    protected virtual void ReceiveSwapRequest(Transform otherPlayer)
    {
        if (_swapHeld)
        {
            _otherPlayer = otherPlayer;

            gameObject.SetActive(false);
            otherPlayer.gameObject.SetActive(false);
            Invoke("Swap", _swapDelay.Value);
        }
    }

    private void Swap()
    {
        Vector3 tempPos = transform.position;
        transform.position = _otherPlayer.position;
        _otherPlayer.position = tempPos;
        gameObject.SetActive(true);
        _otherPlayer.gameObject.SetActive(true);
        _swapCompleted.Raise();
    }

    protected virtual void SwapCompleted()
    {
        _swapHeld = false;
    }

    #endregion

    #region Attacking

    protected virtual void HandleAttacking()
    {
        _playerCombat.Attack();
    }

    #endregion

    #region Horizontal

    protected virtual void HandleHorizontal()
    {
        if (_moveDirection.x != 0)
        {
            float inputX = _moveDirection.x;
            if((inputX > 0 && transform.localScale.x < 0) || (inputX < 0 && transform.localScale.x > 0))
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            _speed.x = Mathf.MoveTowards(_speed.x, inputX * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            _rb.sharedMaterial = _movingMaterial;
        }
        else
        {
            _speed.x = Mathf.MoveTowards(_speed.x, 0, (_grounded ? _stats.GroundDeceleration : _stats.AirDeceleration) * Time.fixedDeltaTime);
            if(_speed.x == 0) _rb.sharedMaterial = _stationaryMaterial;
        }
    }

    #endregion

    #region Jump

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
        _frameJumpWasPressed = _fixedFrame;

        // Double jump
        if (CanDoubleJump)
        {
            _speed.y = _stats.JumpPower;
            _doubleJumpUsable = false;
            _endedJumpEarly = false;
        }

        // Standard jump
        if (CanUseCoyote || HasBufferedJump)
        {
            _coyoteUsable = false;
            _bufferedJumpUsable = false;
            _speed.y = _stats.JumpPower;
        }
    }

    protected virtual void CancelJump()
    {
        // Early end detection
        if (!_endedJumpEarly && !_grounded && _rb.velocity.y > 0) _endedJumpEarly = true;
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

    private bool _canDash;
    private bool _dashing;
    private int _startedDashing;

    protected virtual void StartDash()
    {
        if (_canDash)
        {
            _dashing = true;
            _canDash = false;
            _startedDashing = _fixedFrame;

            // Strip external buildup
            _currentExternalVelocity = Vector2.zero;
        }
    }

    protected virtual void HandleDash()
    {
        if (_dashing)
        {
            _speed = new Vector2(_moveDirection.x * _stats.DashVelocity, _speed.y >= 0 ? 0 : _speed.y);
            // Cancel when the time is out or we've reached our max safety distance
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames)
            {
                _dashing = false;
                _speed.x *= _stats.DashEndHorizontalMultiplier;
                if (_grounded) _canDash = true;
            }
        }
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
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _col.size.y * transform.localScale.y / 2 + _stats.GrounderDistance * 2, ~_stats.PlayerLayer);
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
    }

    private void OnEnable()
    {
        _receiveSwapRequest.Subscribe(ReceiveSwapRequest);
        _swapCompleted.Subscribe(SwapCompleted);
        _actions.Enable();
    }

    private void OnDisable()
    {
        _receiveSwapRequest.UnSubscribe(ReceiveSwapRequest);
        _swapCompleted.UnSubscribe(SwapCompleted);
        _actions.Disable();
    }
}