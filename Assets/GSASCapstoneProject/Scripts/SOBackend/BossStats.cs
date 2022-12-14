///
/// Created by Dennis Chen
///

using UnityEngine;

//List of stats that apply to either one of the bosses or both
[CreateAssetMenu]
public class BossStats : ScriptableObject
{
    [Tooltip("True if the character is the heaven boss")]
    public bool Heaven;

    #region Movement

    [Header("MOVEMENT")]

    #endregion

    #region Weights

    [Header("Attack Weights")]
    public float ShotgunWeight;
    public float RifleWeight;
    public float ThinWallWeight;
    public float ThickWallWeight;
    [BoolAttribute("Heaven", true)]
    public float LaserWeight;
    [BoolAttribute("Heaven", true)]
    public float FeatherWeight;
    [BoolAttribute("Heaven", false)]
    public float SwipeWeight;
    [BoolAttribute("Heaven", false)]
    public float CrushWeight;
    [BoolAttribute("Heaven", false)]
    public float MeteorWeight;

    #endregion

    #region Delay

    [Header("How Long Before the Boss Becomes Idle/Starts Moving Again")]
    public float ShotgunDelay;
    public float RifleDelay;
    public float ThinWallDelay;
    public float ThickWallDelay;
    [BoolAttribute("Heaven", true)]
    public float LaserDelay;
    [BoolAttribute("Heaven", true)]
    public float FeatherDelay;
    [BoolAttribute("Heaven", false)]
    public float SwipeDelay;
    [BoolAttribute("Heaven", false)]
    public float CrushDelay;
    [BoolAttribute("Heaven", false)]
    public float MeteorDelay;

    #endregion

    #region Other

    [Header("Other")]

    public int MinRays;

    public int MaxRays;

    public int BurstBullets;
    #endregion
}
