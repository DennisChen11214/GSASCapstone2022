using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public class DamageModule : MonoBehaviour
{
    [SerializeField]
    FloatVariable _health;

    public void TakeDamage(float damage)
    {
        _health.Value -= damage;
        if (_health.Value <= 0) Destroy(gameObject);
    }
}
