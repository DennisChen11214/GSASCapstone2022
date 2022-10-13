using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject {
    [Tooltip("True if the character is melee")]
    public bool Melee;

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

    [Tooltip("Maximum Number of Dash Charges")]
    public int MaxDashes = 1;
    
    [Tooltip("Cooldown for a dash charge")]
    public float MovementCooldown = 3;

    [Tooltip("The velocity of the dash")] 
    public float DashVelocity = 50;

    [Tooltip("Damage of the attack dash")]
    public float DashDamage = 10;

    [Tooltip("Amount of charge the charge dash gives")]
    public float ChargeGivenDash = 0.3f;

    [Tooltip("Amount of charge the penalty charge takes away")]
    public float PenaltyGivenDash = 0.4f;

    [Tooltip("How many fixed frames the dash will last")]
    public int DashDurationFrames = 5;

    [Tooltip("The horizontal speed retained when dash has completed")]
    public float DashEndHorizontalMultiplier = 0.25f;

    [Tooltip("The vertical speed retained when dash has completed")]
    public float DashEndVerticalMultiplier = 0.25f;

    [Header("COLLISIONS")]
    [Tooltip("The detection distance for grounding and roof detection")]
    public float GrounderDistance = 0.1f;

    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("EXTERNAL")] 
    [Tooltip("The rate at which external velocity decays")]
    public int ExternalVelocityDecay = 100;

    [Header("Combat")]
    [Tooltip("The amount of time before the attack combo resets")]
    public float TimeBeforeAttackResets = 0.3f;

    [Tooltip("The amount of time between attack combos")]
    public float TimeBeforeNextCombo = 2f;

    [Tooltip("How long a player is knocked back for")]
    public float KnockbackLength = 0.2f;

    [Tooltip("How much of the original horizontal knockback speed is retained when starting to fall")]
    public float KnockbackFactor = 0.33f;

    [Tooltip("The amount of knockback the player takes when hit")]
    public float Knockback = 10;

    [Tooltip("Number of projectiles the ranged character spawns for the 1st attack")]
    public int NumProjectilesAttack1 = 4;

    [Tooltip("Number of projectiles the ranged character spawns for the 2nd attack")]
    public int NumProjectilesAttack2 = 5;

    [Tooltip("Number of projectiles the ranged character spawns for the 3rd attack")]
    public int NumProjectilesAttack3 = 7;

    [Tooltip("Maximum variation in angle from the horizontal for each projectile shot")]
    public float AngleOfVariation = 45;

    [Tooltip("How Long the Ranged Character Takes to Charge the attack")]
    public float ChargeTime = 3;

}
