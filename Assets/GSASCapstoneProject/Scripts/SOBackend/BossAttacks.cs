using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

[CreateAssetMenu]
public class BossAttacks : ScriptableObject
{
    public enum BossAttack
    {
        Shotgun,
        Rifle,
        ThinWall,
        ThickWall,
        Lasers,
        Feathers,
        Swipe,
        Crush,
        Meteors
    }

    public List<BossAttack> AttackCombo;
    public float HealthThreshold;
    public bool done;
}
