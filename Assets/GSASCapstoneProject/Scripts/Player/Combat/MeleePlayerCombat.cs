using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public class MeleePlayerCombat : PlayerCombat
{
    [SerializeField]
    FloatVariable _bossHealth;

    public override void Attack()
    {

    }

    public override void TakeDamage(float damage)
    {
        _playerHealth.Value -= damage;
    }
}
