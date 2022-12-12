///
/// Created by Dennis Chen
///

using System.Collections.Generic;
using UnityEngine;

//Used if the designer wants to have the boss do a certain attack combo at a certain health threshold
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
