using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public abstract class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    protected FloatVariable _playerHealth;

    [SerializeField]
    protected ScriptableStats _stats;

    public abstract void Attack();
    public abstract void TakeDamage(float damage);
}
