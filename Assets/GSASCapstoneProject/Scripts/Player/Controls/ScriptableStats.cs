using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject {
    [Header("MOVEMENT")] 
    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on vertical moving platforms and slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Header("CROUCHING")] 
    [Tooltip("A dead-zone for controllers to prevent unintended crouching")]
    public float CrouchInputThreshold = -0.5f;

    [Tooltip("A speed multiplier while crouching"), Range(0f, 1f)]
    public float CrouchSpeedPenalty = 0.5f;

    [Tooltip("The amount of frames it takes to hit the full crouch speed penalty. Higher values provide more crouch sliding")]
    public int CrouchSlowdownFrames = 50;

    [Tooltip("Detection height offset from the top of the standing collider. Smaller values risk collisions when standing up")]
    public float CrouchBufferCheck = 0.1f;

    [Header("JUMP")] 
    [Tooltip("Enable double jump")]
    public bool AllowDoubleJump;

    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 40;

    [Tooltip("The player's capacity to gain fall speed")]
    public float FallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The fixed frames before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public int CoyoteFrames = 7;

    [Tooltip("The amount of fixed frames we buffer a jump. This allows jump input before actually hitting the ground")]
    public int JumpBufferFrames = 7;

    [Header("DASH")] 
    [Tooltip("Allows the player to dash")]
    public bool AllowDash = true;

    [Tooltip("The velocity of the dash")] 
    public float DashVelocity = 50;

    [Tooltip("How many fixed frames the dash will last")]
    public int DashDurationFrames = 5;

    [Tooltip("The horizontal speed retained when dash has completed")]
    public float DashEndHorizontalMultiplier = 0.25f;
        
    [Header("WALLS")] 
    [Tooltip("Allow wall sliding & jumping")]
    public bool AllowWalls = true;
        
    [Tooltip("Only wall slide when you're physically pushing against the wall")]
    public bool RequireInputPush = false;

    [Tooltip("How fast you climb walls.")]
    public float WallClimbSpeed = 10;

    [Tooltip("Bounds for detecting walls on either side. Ensure it horizontally overflows your collider")]
    public Vector2 WallDetectorSize = new(0.75f,1);
        
    [Tooltip("Set this to the layer climbable walls are on")]
    public LayerMask ClimbableLayer;
        
    [Tooltip("The player's capacity to gain wall sliding speed. 0 = stick to wall")]
    public float WallFallAcceleration = 8;

    [Tooltip("Clamps the maximum fall speed")]
    public float MaxWallFallSpeed = 8;

    [Tooltip("The immediate velocity horizontal velocity applied when wall jumping")]
    public Vector2 WallJumpPower = new(30, 25);
        
    [Tooltip("The frames before full horizontal movement is returned after a wall jump")]
    public int WallJumpInputLossFrames = 20;
        
    [Header("COLLISIONS")]
    [Tooltip("The detection distance for grounding and roof detection")]
    public float GrounderDistance = 0.1f;

    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("ATTACK")] 
    [Tooltip("The fixed frame cooldown of your players basic attack")]
    public int AttackFrameCooldown = 15;
        
    [Header("LADDERS")]
    [Tooltip("Allow climbing ladders")]
    public bool SnapToLadders = true;
        
    [Tooltip("Ladder snap speed")]
    public float LadderSnapSpeed = 0.05f;
        
    [Tooltip("Speed at which you shimmy on a ladder")]
    public float LadderShimmySpeedMultiplier = 0.5f;
        
    [Tooltip("Set this to the layer your ladders are on")]
    public LayerMask LadderLayer;
        
    [Tooltip("How fast you climb ladders")]
    public float LadderClimbSpeed = 8;

    [Tooltip("How fast you climb ladders")]
    public float LadderSlideMultiplier = 1.5f;
        
    [Tooltip("How many frames can pass between ladder interactions. Helps jittery movement and jumping glitches")]
    public int LadderCooldownFrames = 8;
        
    [Tooltip("Input required before you mount the ladder. Avoids unwanted climbing using controllers")]
    public float LadderClimbThreshold = .3f;

    [Header("EXTERNAL")] 
    [Tooltip("The rate at which external velocity decays")]
    public int ExternalVelocityDecay = 100;

    [Header("GIZMOS")] 
    [Tooltip("Color: White")]
    public bool ShowWallDetection = true;
}
