using System.Collections;
using System.Collections.Generic;
using Core.GlobalEvents;
using UnityEngine;
using Core.GlobalVariables;
using UnityEngine.Serialization;

public class DamageModule : MonoBehaviour
{
    [SerializeField]
    FloatVariable _health;

    [SerializeField] GlobalEvent _bossDestroy;

    public void TakeDamage(float damage)
    {
        _health.Value -= damage;
        if (_health.Value <= 0) _bossDestroy.Raise();
    }
}
