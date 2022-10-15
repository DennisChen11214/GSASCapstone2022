///
/// Created by Dennis Chen
/// Reference: Tarodev's code talked about in https://www.youtube.com/watch?v=3sWTzMsmdx8
///

using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject {
    [Tooltip("True if the character is melee")]
    public bool Melee;

    #region Movement

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

    #endregion

    #region Jump

    [Header("JUMP")] 
    [Tooltip("Enable double jump")]
    public bool AllowDoubleJump = false;

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

    #endregion

    #region Dash

    [Header("DASH")] 
    
    [Tooltip("Maximum Number of Dash Charges")]
    public int MaxDashes = 1;
    
    [Tooltip("Cooldown for a dash charge")]
    public float MovementCooldown = 3;

    [Tooltip("The velocity of the dash")] 
    public float DashVelocity = 50;

    [Tooltip("How many fixed frames the dash will last")]
    public int DashDurationFrames = 5;

    [Tooltip("How many I-Frames the Dodge Has")]
    public int DashInvincibleFrames = 5;

    [Tooltip("The horizontal speed retained when dash has completed")]
    public float DashEndHorizontalMultiplier = 0.25f;

    [Tooltip("The vertical speed retained when dash has completed")]
    public float DashEndVerticalMultiplier = 0.25f;

    [Tooltip("Damage of the attack dash")]
    [BoolAttribute("Melee", true)]
    public float DashDamage = 10;

    [BoolAttribute("Melee", false)]
    [Tooltip("How far the teleport goes")]
    public float TeleportDistance = 5;

    [BoolAttribute("Melee", false)]
    [Tooltip("What layers the vertical teleport works on")]
    public LayerMask TeleportLayers;

    [BoolAttribute("Melee", false)]
    [Tooltip("Amount of charge the charge dash gives")]
    public float ChargeGivenDash = 0.3f;

    [BoolAttribute("Melee", false)]
    [Tooltip("Amount of charge the penalty dash gives (NEGATIVE)")]
    public float PenaltyGivenDash = -0.4f;

    #endregion

    #region Collisions

    [Header("COLLISIONS")]
    [Tooltip("The detection distance for grounding and roof detection")]
    public float GrounderDistance = 0.1f;

    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    #endregion

    [Header("EXTERNAL")] 
    [Tooltip("The rate at which external velocity decays")]
    public int ExternalVelocityDecay = 100;

    #region Combat

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

    [Tooltip("How much damage the melee character's first attack does")]
    [BoolAttribute("Melee", true)]
    public float MeleeDamageAttack1 = 3;

    [Tooltip("How much damage the melee character's second attack does")]
    [BoolAttribute("Melee", true)]
    public float MeleeDamageAttack2 = 5;

    [Tooltip("How much damage the melee character's third attack does")]
    [BoolAttribute("Melee", true)]
    public float MeleeDamageAttack3 = 8;

    [Tooltip("How much damage the melee character's vertical attack does")]
    [BoolAttribute("Melee", true)]
    public float MeleeDamageVerticalAttack = 5;

    [Tooltip("Number of projectiles the ranged character spawns for the 1st attack")]
    [BoolAttribute("Melee", false)]
    public int NumProjectilesAttack1 = 4;

    [Tooltip("Number of projectiles the ranged character spawns for the 2nd attack")]
    [BoolAttribute("Melee", false)]
    public int NumProjectilesAttack2 = 5;

    [Tooltip("Number of projectiles the ranged character spawns for the 3rd attack")]
    [BoolAttribute("Melee", false)]
    public int NumProjectilesAttack3 = 7;

    [Tooltip("How fast the projectile moves")]
    [BoolAttribute("Melee", false)]
    public float ProjectileSpeed = 30;

    [Tooltip("How long the projectile lasts before breaking")]
    [BoolAttribute("Melee", false)]
    public float ProjectileDuration = 0.5f;

    [Tooltip("How much damage the ranged character's projectile does")]
    [BoolAttribute("Melee", false)]
    public float ProjectileDamage = 1;

    [Tooltip("Maximum variation in angle from the horizontal for each projectile shot")]
    [BoolAttribute("Melee", false)]
    public float AngleOfVariation = 45;

    [Tooltip("How Long the Ranged Character Takes to Charge the attack")]
    [BoolAttribute("Melee", false)]
    public float ChargeTime = 3;

    [Tooltip("How Much Damage the Charge Attack Does")]
    [BoolAttribute("Melee", false)]
    public float ChargeBeamDamage = 20;

    [Tooltip("How Much Slower the Player Moves When Charging")]
    [BoolAttribute("Melee", false)]
    public float ChargeSlow = 0.7f;

    #endregion
}
