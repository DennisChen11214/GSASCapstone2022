///
/// Created by Dennis Chen
/// Reference: Tarodev's code talked about in https://www.youtube.com/watch?v=3sWTzMsmdx8
///

using Core.GlobalEvents;
using Core.GlobalVariables;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    //Different possible dash types used during prototyping
    public enum MeleeDashType
    {
        DodgeDash,
        OmniDir,
        AttackDash,
    }

    public enum RangedDashType
    {
        OmniDir,
        Charge,
        Penalty,
        Vertical
    }

    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private PhysicsMaterial2D _movingMaterial;
    [SerializeField] private PhysicsMaterial2D _stationaryMaterial;
    [BoolAttribute("Melee", true, "_stats")]
    [SerializeField] private MeleeDashType _meleeDashType;
    [BoolAttribute("Melee", false, "_stats")]
    [SerializeField] private RangedDashType _rangedDashType;
    [SerializeField] private LayerMask _bossLayer;
    [SerializeField] private LayerMask _platformLayer;
    [SerializeField] private LayerMask _respawnLayer;
    [SerializeField] private AudioClip _swapSound;
    [SerializeField] private GlobalEvent _swapCompleted;
    [SerializeField] private GlobalEvent _swapCanceled;
    [BoolAttribute("Melee", true, "_stats")]
    [SerializeField] private GlobalEvent _onDownAttackInAir;
    [SerializeField] private GlobalEvent _onOtherPlayerRevived;
    [SerializeField] private GlobalEvent _onGamePaused;
    [SerializeField] private GlobalEvent _onGameUnpaused;
    [SerializeField] private TransformGlobalEvent _requestSwap;
    [SerializeField] private TransformGlobalEvent _receiveSwapRequest;
    [SerializeField] private TransformVariable _playerTransform;
    [SerializeField] private BoolVariable _isKnockedBack;
    [SerializeField] private BoolVariable _isInvincible;
    [BoolAttribute("Melee", false, "_stats")]
    [SerializeField] private BoolVariable _isCharging;
    [SerializeField] private BoolVariable _isOtherPlayerDead;
    [SerializeField] private BoolVariable _isAttacking;
    [SerializeField] private FloatVariable _swapDelay;
    [SerializeField] private FloatVariable _movementCooldown;
    [BoolAttribute("Melee", true, "_stats")]
    [SerializeField] private FloatVariable _floatTime;
    [SerializeField] private IntVariable _numDashes;


    #region Internal

    private Rigidbody2D _rb;
    private PlayerCombat _playerCombat;
    private BoxCollider2D _col;
    private AudioSource _audio;

    private Vector2 _moveDirection;
    private Vector2 _speed;
    private Vector2 _currentExternalVelocity;
    private Transform _otherPlayer;
    private int _fixedFrame;
    private bool _grounded;
    private bool _swapPressedBeforePause;

    private InputActionAsset _actions;
    private InputAction _move, _jump, _dash, _attack, _swap, _pause;

    #endregion

    #region External
    public ScriptableStats PlayerStats => _stats;
    public Vector2 Input => _moveDirection;
    public Vector2 Speed => _speed;
    public Vector2 GroundNormal => _groundNormal;

    [HideInInspector]
    public bool IsShootingLaser;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        _playerCombat = GetComponent<PlayerCombat>();
        _audio = transform.parent.GetComponent<AudioSource>();
        _actions = GetComponentInParent<PlayerInput>().actions;

        _playerTransform.Value = transform;

        _move = _actions.FindActionMap("Player").FindAction("Movement");
        _jump = _actions.FindActionMap("Player").FindAction("Jump");
        _dash = _actions.FindActionMap("Player").FindAction("Movement Ability");
        _attack = _actions.FindActionMap("Player").FindAction("Attack");
        _swap = _actions.FindActionMap("Player").FindAction("Swap");
        _pause = _actions.FindActionMap("Player").FindAction("Pause");

        _onGamePaused.Subscribe(Pause);
        _onGameUnpaused.Subscribe(Unpause);

        //Map the input actions
        _attack.started += HandleAttacking;
        _attack.canceled += CancelCharging;
        _attack.performed += TryRevive;
        _jump.started += HandleJump;
        _jump.canceled += CancelJump;
        _dash.performed += StartDash;
        _swap.started += RequestSwap;
        _swap.canceled += CancelSwap;
        _pause.performed += Pause;

        _numDashes.Value = _stats.MaxDashes;
        _movementCooldown.Value = _stats.MovementCooldown;
    }

    private void FixedUpdate()
    {
        _fixedFrame++;
        _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);
        //Depending on the movement input, change the movement direction vector
        if (_move.ReadValue<Vector2>().magnitude > 0.1f)
        {
            _moveDirection = _move.ReadValue<Vector2>().normalized;
            _moveDirection = AngleToDirection(Vector2.Angle(Vector2.right, _moveDirection), _moveDirection.y > 0, _moveDirection.x > 0);
        }
        else 
        {
            _moveDirection = Vector2.zero;
        }
        CheckCollisions();

        HandleCollisions();
        HandleHorizontal();
        HandleDash();
        HandleFall();

        ApplyVelocity();
    }

    //Used to convert the joystick numbers to a raw axis vector
    private static Vector2 AngleToDirection(float angle, bool up, bool right)
    {
        int vertFactor = up ? 1 : -1;
        int horFactor = right ? 1 : -1;
        if (angle >= 22.5f && angle < 67.5f)
        {
            return new Vector2(1, 1 * vertFactor);
        }
        else if (angle >= 67.5f && angle < 112.5)
        {
            return new Vector2(0, 1 * vertFactor);
        }
        else if (angle >= 112.5 && angle < 157.5)
        {
            return new Vector2(-1, 1 * vertFactor);
        }
        else
        {
            return new Vector2(1 * horFactor, 0);
        }
    }

    #region Collisions

    private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
    private int _groundHitCount;
    private int _frameLeftGrounded = int.MinValue;

    //Checks if the player is on the floor
    private void CheckCollisions()
    {
        Physics2D.queriesHitTriggers = false;
        Vector2 origin = (Vector2)transform.position + _col.offset * transform.localScale;
        Vector2 _absScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        _groundHitCount = Physics2D.BoxCastNonAlloc(origin, _col.size * _absScale, 0, Vector2.down, _groundHits, _stats.GrounderDistance, ~_stats.PlayerLayer);

        //If the player downjumped through a platform and is no longer colliding with it, set the platform collider back to normal
        if (_downJumpCollider != null)
        {
            List<Collider2D> results = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(_platformLayer);
            Physics2D.OverlapCollider(_col, contactFilter, results);
            if(!results.Contains(_downJumpCollider))
            {
                _downJumpCollider.gameObject.GetComponent<PlatformEffector2D>().rotationalOffset = 0;
                _downJumpCollider = null;
            }
        }
        Physics2D.queriesHitTriggers = true;
    }

    //Handles the player jumping off the ground and landing back on the ground
    private void HandleCollisions()
    {
        // Landed on the Ground
        if (!_grounded && _groundHitCount > 0)
        {
            _grounded = true;
            _canDash = true;
            _isKnockedBack.Value = false;
            Invoke("TurnOffInvincibility", _stats.InvincibleTime);
            ResetJump();
        }
        // Left the Ground
        else if (_grounded && _groundHitCount == 0)
        {
            _grounded = false;
            _frameLeftGrounded = _fixedFrame;
        }
    }

    //The invincibility after getting knocked back is turned off after a certain amount of time from hitting the ground
    private void TurnOffInvincibility()
    {
        if (!_dashing)
        {
            _isInvincible.Value = false;
        }
    }

    #endregion

    #region Swapping
    private bool _swapHeld;

    //Occurs if one of the players initiates a swap
    private void RequestSwap(InputAction.CallbackContext ctx)
    {
        if (_isOtherPlayerDead.Value) return;
        _swapHeld = true;
        _requestSwap.Raise(transform);
    }

    //Cancels a swap request
    private void CancelSwap(InputAction.CallbackContext ctx)
    {
        _swapHeld = false;
        _swapCanceled.Raise();
    }

    //If the other player requests a swap, check if this player also wants to swap
    private void ReceiveSwapRequest(Transform otherPlayer)
    {
        //Swap both players if they are both holding the swap button
        if (_swapHeld)
        {
            _otherPlayer = otherPlayer;

            _audio.PlayOneShot(_swapSound);
            //Sets the 2 players inactive for a certain amount of time before swapping them
            gameObject.SetActive(false);
            otherPlayer.gameObject.SetActive(false);
            Invoke("Swap", _swapDelay.Value);
        }
    }

    //Swap the positions of both players
    private void Swap()
    {
        Vector3 tempPos = transform.position;
        transform.position = _otherPlayer.position;
        _otherPlayer.position = tempPos;
        gameObject.SetActive(true);
        _otherPlayer.gameObject.SetActive(true);
        _swapCompleted.Raise();
    }

    private void SwapCompleted()
    {
        _swapHeld = false;
    }

    #endregion

    #region Attacking

    //Handles vertical and normal attacks
    private void HandleAttacking(InputAction.CallbackContext ctx)
    {
        if (!_isKnockedBack.Value)
        {
            if(Mathf.Abs(_moveDirection.y) > 0 && _moveDirection.x == 0 && _stats.Melee)
            {
                _playerCombat.VerticalAttack(_moveDirection.y > 0);
            }
            else
            {
                _playerCombat.Attack();
            }
        }
    }

    //When interacting with a tombstone, revive the other player
    private void TryRevive(InputAction.CallbackContext ctx)
    {
        if (_isOtherPlayerDead.Value)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(_respawnLayer);
            contactFilter.useTriggers = true;
            List<Collider2D> colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(_col, contactFilter, colliders);
            if(colliders.Count > 0)
            {
                _onOtherPlayerRevived.Raise();
                _isOtherPlayerDead.Value = false;
            }
        }
    }

    private void CancelCharging(InputAction.CallbackContext ctx)
    {
        _playerCombat.CancelAttack();
    }

    //Adds an upwards bounce to a player in midair
    private void DownAttackAirBounce()
    {
        _speed.y = _stats.DownAttackBounceSpeed;
    }

    #endregion

    #region Horizontal

    //Handles the horizontal movement input from the player
    private void HandleHorizontal()
    {
        if (_isKnockedBack.Value) return;
        if (_moveDirection.x != 0 && !IsShootingLaser)
        {
            float inputX = _moveDirection.x;
            if(!_isAttacking.Value && ((inputX > 0 && transform.localScale.x < 0) || (inputX < 0 && transform.localScale.x > 0)))
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

    //Checks for the player turning around
    public void CheckForFlip()
    {
        if (_isKnockedBack.Value) return;
        if (_moveDirection.x != 0 && !IsShootingLaser)
        {
            float inputX = _moveDirection.x;
            if ((inputX > 0 && transform.localScale.x < 0) || (inputX < 0 && transform.localScale.x > 0))
            {

                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    #endregion

    #region Jump

    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private bool _doubleJumpUsable;
    private bool _bufferedJumpUsable;
    private Collider2D _downJumpCollider;
    private int _frameJumpWasPressed = int.MinValue;

    private bool CanUseCoyote => _coyoteUsable && !_grounded && _fixedFrame < _frameLeftGrounded + _stats.CoyoteFrames;
    private bool HasBufferedJump => _grounded && _bufferedJumpUsable && _fixedFrame < _frameJumpWasPressed + _stats.JumpBufferFrames;
    private bool CanDoubleJump => _stats.AllowDoubleJump && _doubleJumpUsable && !_coyoteUsable;

    //Handles all the jumps a player can do, including buffered and down jumps.
    //Jumps have a coyote effect, where players who just left a platform can still jump
    private void HandleJump(InputAction.CallbackContext ctx)
    {
        if (_isKnockedBack.Value || _dashing || IsShootingLaser) return;

        _frameJumpWasPressed = _fixedFrame;

        // Double jump
        /*if (CanDoubleJump)
        {
            _speed.y = _stats.JumpPower;
            _doubleJumpUsable = false;
            _endedJumpEarly = false;
        }*/

        //Down jump through platform
        if (_grounded && _moveDirection.y == -1)
        {
            DownJump();
        }
        // Standard jump
        else if (CanUseCoyote || HasBufferedJump)
        {
            _coyoteUsable = false;
            _bufferedJumpUsable = false;
            _speed.y = _stats.JumpPower;
        }
    }

    //Changes the collider of the platform to allow the player to jump down it
    private void DownJump()
    {
        Vector2 origin = (Vector2)transform.position + _col.offset * transform.localScale;
        RaycastHit2D hit2D = Physics2D.Raycast(origin, Vector2.down, 5, _platformLayer);
        _downJumpCollider = hit2D.collider;
        if(_downJumpCollider != null)
        {
            _downJumpCollider.gameObject.GetComponent<PlatformEffector2D>().rotationalOffset = 180;   
            _coyoteUsable = false;
            _bufferedJumpUsable = false;
        }
    }

    private void CancelJump(InputAction.CallbackContext ctx)
    {
        // Early end detection
        if (!_endedJumpEarly && !_grounded && !_dashing && _rb.velocity.y > 0) _endedJumpEarly = true;
    }


    private void ResetJump()
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
    private Vector2 _dashVel;

    //Initializes variables for the player to start dashing if they aren't already doing so
    private void StartDash(InputAction.CallbackContext ctx)
    {
        if (_canDash && _numDashes.Value > 0 && !_isKnockedBack.Value && !IsShootingLaser)
        {
            _dashing = true;
            _canDash = false;
            _numDashes.Value--;
            _startedDashing = _fixedFrame;
            _dashVel = _moveDirection;

            // Strip external buildup
            _currentExternalVelocity = Vector2.zero;

            _dealtDamageThisDash = false;
        }
    }

    //Dashes if one is available according to the set dash-type
    private void HandleDash()
    {
        //Increases the amount of dashes the player can use if the cooldown has passed and we don't have the maximum amount of dashes
        if(_numDashes.Value < _stats.MaxDashes)
        {
            if (_movementCooldown.Value > 0)
            {
                _movementCooldown.Value -= Time.fixedDeltaTime;
            }
            else
            {
                _numDashes.Value++;
                _movementCooldown.Value = _stats.MovementCooldown;
            }
        }
        if (_stats.Melee)
        {
            switch (_meleeDashType)
            {
                case MeleeDashType.DodgeDash:
                    MeleeDodgeDash();
                    break;
                case MeleeDashType.OmniDir:
                    MeleeOmniDash();
                    break;
                case MeleeDashType.AttackDash:
                    MeleeAttackDash();
                    break;
            }
        }
        else
        {
            switch (_rangedDashType)
            {
                case RangedDashType.OmniDir:
                    RangedOmniTeleport();
                    break;
                case RangedDashType.Charge:
                    RangedChargeDash(_stats.ChargeGivenDash);
                    break;
                case RangedDashType.Penalty:
                    RangedChargeDash(_stats.PenaltyGivenDash);
                    break;
                case RangedDashType.Vertical:
                    RangedVerticalTeleport();
                    break;
            }
        }
    }

    //Horizontal dash with i-frames throughout the whole dash
    private void MeleeDodgeDash()
    {
        _stats.DashDurationFrames = 9;
        if (_dashing)
        {
            _isInvincible.Value = true;
            _speed = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0) * _stats.DashVelocity;
            // Cancel when the time is out or we've reached our max safety distance
            if(_fixedFrame > _startedDashing + _stats.DashInvincibleFrames)
            {
                _isInvincible.Value = false;
            }
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames)
            {
                _dashing = false;
                _speed.x *= _stats.DashEndHorizontalMultiplier;
                _canDash = true;
            }
        }
    }

    //Omni-directional dash with partial i-frames
    private void MeleeOmniDash()
    {
        _stats.DashDurationFrames = 7;
        if (_dashing)
        {
            _isInvincible.Value = true;
            if (_dashVel == Vector2.zero)
            {
                _speed = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0) * _stats.DashVelocity;
            }
            else
            {
                _speed = new Vector2(_dashVel.x, _dashVel.y).normalized * _stats.DashVelocity;
            }
            if (_fixedFrame > _startedDashing + _stats.DashInvincibleFrames)
            {
                _isInvincible.Value = false;
            }
            // Cancel when the time is out or we've reached our max safety distance
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames)
            {
                _dashing = false;
                _speed.x *= _stats.DashEndHorizontalMultiplier;
                _speed.y *= _stats.DashEndVerticalMultiplier;
                _canDash = true;
            }
        }
    }

    private bool _dealtDamageThisDash;

    //Dashes horizontally, dealing damage to any enemies in the path
    private void MeleeAttackDash()
    {
        _stats.DashDurationFrames = 9;
        if (_dashing)
        {
            _isInvincible.Value = true;
            _speed = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0) * _stats.DashVelocity;
            // Cancel when the time is out or we've reached our max safety distance
            if (_fixedFrame > _startedDashing + _stats.DashInvincibleFrames)
            {
                _isInvincible.Value = false;
            }
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames)
            {
                _dashing = false;
                _speed.x *= _stats.DashEndHorizontalMultiplier;
                _canDash = true;
            }
        }
    }

    //Deals damage to a boss touched during the attack dash
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_dashing && _stats.Melee && _meleeDashType == MeleeDashType.AttackDash 
            && !_dealtDamageThisDash && _bossLayer == (_bossLayer | (1 << collision.gameObject.layer)))
        {
            _dealtDamageThisDash = true;
            collision.gameObject.GetComponent<DamageModule>().TakeDamage(_stats.DashDamage);
        }
    }

    //Omni-directional teleport
    private void RangedOmniTeleport()
    {
        if (_dashing)
        {
            if (_dashVel == Vector2.zero)
            {
                transform.position += new Vector3(transform.localScale.x > 0 ? 1 : -1, 0, 0) * _stats.TeleportDistance;
            }
            else
            {
                transform.position += new Vector3(_dashVel.x, _dashVel.y, 0).normalized * _stats.TeleportDistance;
            }
            _dashing = false;
            _speed.y = 0;
            _canDash = true;
            CheckCollisions();
            HandleCollisions();
        }
    }

    //Teleports in a horizontal direction while also increasing the charge of the charge beam
    private void RangedChargeDash(float percent)
    {
        _stats.DashDurationFrames = 9;
        if (_dashing)
        {
            _isInvincible.Value = true;
            _speed = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0) * _stats.DashVelocity;
            // Cancel when the time is out or we've reached our max safety distance
            if (_fixedFrame > _startedDashing + _stats.DashInvincibleFrames)
            {
                _isInvincible.Value = false;
            }
            if (_fixedFrame > _startedDashing + _stats.DashDurationFrames)
            {
                _dashing = false;
                _speed.x *= _stats.DashEndHorizontalMultiplier;
                _playerCombat.IncreaseCharge(percent);
                _canDash = true;
            }
        }
    }
    
    //If a platform above or below the player exists, then teleport on top of that platform
    private void RangedVerticalTeleport()
    {
        if (_dashing)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.up * _dashVel.y, 50, _stats.TeleportLayers);
            if(_dashVel.y == 0 || hit2D.collider == null)
            {
                _dashing = false;
                _canDash = true;
                _numDashes.Value++;
                _movementCooldown.Value = _stats.MovementCooldown;
                return;
            }
            Vector2 teleportPos = hit2D.point;
            teleportPos.y += ((BoxCollider2D)hit2D.collider).size.y * hit2D.collider.gameObject.transform.lossyScale.y;
            teleportPos.y += _col.size.y / 2 - _col.offset.y;
            transform.position = teleportPos;
            _dashing = false;
            _canDash = true;
            CheckCollisions();
            HandleCollisions();
        }
    }

    #endregion

    #region Falling

    private Vector2 _groundNormal;
    private bool _isFallingAfterKnockback;

    //Handles how movement on slopes are done and how velocity changes when falling through the air
    private void HandleFall()
    {
        if (_dashing) return;

        // Grounded & Slopes
        if (_grounded && _speed.y <= 0f)
        {
            _speed.y = _stats.GroundingForce;

            // Raycast downwards to check if we hit the ground
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

        if(_stats.Melee && _floatTime.Value > 0)
        {
            _floatTime.Value -= Time.fixedDeltaTime;
            _speed.x *= 0.5f;
            _speed.y = 0;
            return;
        }
        
        // In Air
        var fallSpeed = _stats.FallAcceleration;
        if (_endedJumpEarly && _speed.y > 0) fallSpeed *= _stats.JumpEndEarlyGravityModifier;
        _speed.y = Mathf.MoveTowards(_speed.y, -_stats.MaxFallSpeed, fallSpeed * Time.fixedDeltaTime);
    }

    #endregion

    private void Pause(InputAction.CallbackContext ctx)
    {
        _onGamePaused.Raise();
    }

    //Set the velocity of the rigidbody according to the calculated speed and other factors
    private void ApplyVelocity()
    {
        if (!_isKnockedBack.Value)
        {
            _isFallingAfterKnockback = false;
            _rb.velocity = _speed + _currentExternalVelocity;
            //Move slower if charging a beam attack
            if (!_stats.Melee && _isCharging.Value)
            {
                _rb.velocity *= _stats.ChargeSlow;
            }
        }
        else if(_rb.velocity.y < 0 )
        {
            if (!_isFallingAfterKnockback)
            {
                _speed = Vector2.zero;
                _isFallingAfterKnockback = true;
            }
            _rb.velocity = new Vector2(_rb.velocity.x, _speed.y);
        }
    }

    private void OnEnable()
    {
        _receiveSwapRequest.Subscribe(ReceiveSwapRequest);
        _swapCompleted.Subscribe(SwapCompleted);
        if (_stats.Melee)
        {
            _onDownAttackInAir.Subscribe(DownAttackAirBounce);
        }
        _actions.Enable();
    }

    private void OnDisable()
    {
        _receiveSwapRequest.Unsubscribe(ReceiveSwapRequest);
        _swapCompleted.Unsubscribe(SwapCompleted);
        if (_stats.Melee)
        {
            _onDownAttackInAir.Unsubscribe(DownAttackAirBounce);
        }
        _actions.Disable();
        _pause.Enable();
    }

    private void Pause()
    {
        _attack.started -= HandleAttacking;
        _attack.canceled -= CancelCharging;
        _attack.performed -= TryRevive;
        _jump.started -= HandleJump;
        _jump.canceled -= CancelJump;
        _dash.performed -= StartDash;
        _swap.started -= RequestSwap;
        _swap.canceled -= CancelSwap;
        _pause.performed -= Pause;

        _swapPressedBeforePause = _swap.IsPressed();
    }

    private void Unpause()
    {
        _attack.started += HandleAttacking;
        _attack.canceled += CancelCharging;
        _attack.performed += TryRevive;
        _jump.started += HandleJump;
        _jump.canceled += CancelJump;
        _dash.performed += StartDash;
        _swap.started += RequestSwap;
        _swap.canceled += CancelSwap;
        _pause.performed += Pause;

        if (!_attack.IsPressed())
        {
            _playerCombat.CancelAttack();
        }
        if (!_swap.IsPressed() && _swapPressedBeforePause)
        {
            _swapHeld = false;
            _swapCanceled.Raise();
        }
        _swapPressedBeforePause = false;
    }

    private void OnDestroy()
    {
        _attack.started -= HandleAttacking;
        _attack.canceled -= CancelCharging;
        _attack.performed -= TryRevive;
        _jump.started -= HandleJump;
        _jump.canceled -= CancelJump;
        _dash.performed -= StartDash;
        _swap.started -= RequestSwap;
        _swap.canceled -= CancelSwap;
        _pause.performed -= Pause;

        _onGamePaused.Unsubscribe(Pause);
        _onGameUnpaused.Unsubscribe(Unpause);
    }
}