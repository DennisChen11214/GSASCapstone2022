using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public class ScratchAttackModule : MonoBehaviour
{
    [SerializeField] private Scratch _scratch;
    private Scratch[] _pool;
    [SerializeField] private int _poolSize = 4;
    [SerializeField] private Transform _poolParent;

    [SerializeField] private BoolVariable _isBossEnraged;
    
    

    private void Start()
    { 
        // Instantiate attacks (projectiles) in the pool
        _pool = new Scratch[_poolSize];
        if (_poolParent == null)
        {
            _poolParent = transform;
        }
        for(int i = 0; i < _poolSize; i++)
        {
            _pool[i] = Instantiate<Scratch>(_scratch, transform.position, Quaternion.identity, _poolParent);
            _pool[i].gameObject.SetActive(false);
        }
    }
    
    
    public void ScratchAttack(Vector2 position)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
            {
                _pool[i].transform.position = position;
                _pool[i].gameObject.SetActive(true);
                _pool[i].GetComponent<Scratch>().ScratchAttack();
                break;
            }
        }
    }
}
