using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Not being used(for now)
public abstract class AttackModule : MonoBehaviour
{
    [SerializeField] private int _poolSize;
    [SerializeField] private Transform _poolParent;
    [SerializeField] private bool _isAttacking;

    public bool CanAttack()
    {
        return !_isAttacking;
    }
    
    
}
