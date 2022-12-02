using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public class BulletAttackModule : MonoBehaviour
{
    [SerializeField] private Bullet _bullet;
    private Bullet[] _pool;
    [SerializeField] private int _poolSize;
    [SerializeField] private Transform _poolParent;

    [SerializeField] private Transform target;
    [SerializeField] private Transform bossPart;
    [SerializeField] private BoolVariable _isBossEnraged;
    [SerializeField] private FloatVariable _enragedBulletMultiplier;
    
    public bool doneAttacking;
    
    

    private void Start()
    { 
        // Instantiate attacks (projectiles) in the pool
        _pool = new Bullet[_poolSize];
        if (_poolParent == null)
        {
            _poolParent = transform;
        }
        for(int i = 0; i < _poolSize; i++)
        {
            _pool[i] = Instantiate<Bullet>(_bullet, transform.position, Quaternion.identity, _poolParent);
            _pool[i].gameObject.SetActive(false);
        }

        doneAttacking = true;
    }

    public void SetTarget(Transform tr)
    {
        target = tr;
    }

    public void SetBossPart(Transform boss)
    {
        bossPart = boss;
    }
    
    public void Attack(Vector2 position, Vector2 direction)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
            {
                _pool[i].transform.position = position;
                _pool[i].gameObject.SetActive(true);
                _pool[i].Launch(direction);
                break;
            }
        }
    }
    
    
    public void Burst(int numBullets)
    {
        if (_isBossEnraged.Value)
        {
            numBullets = Mathf.CeilToInt(numBullets * _enragedBulletMultiplier.Value);
        }
        if (doneAttacking)
        {
            StartCoroutine(_Burst(numBullets));
        }
    }
    
    private IEnumerator _Burst(int numBullets)
    {
        doneAttacking = false;
        var position = bossPart.position;
        Vector2 originalDir = (target.position - position);
        Vector2 dir = originalDir;
        
        for(int i = 0; i < numBullets; i++)
        {
            Attack(position, dir.normalized);
            dir = originalDir + new Vector2(0, UnityEngine.Random.Range(-1.0f, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }
        
        doneAttacking = true;
    }

    // Rifle
    public void RifleBurst(int numBullets)
    {
        if (_isBossEnraged.Value)
        {
            numBullets = Mathf.CeilToInt(numBullets * _enragedBulletMultiplier.Value);
        }
        if (doneAttacking)
        {
            StartCoroutine(_RifleBurst(numBullets));
        }
    }
    
    private IEnumerator _RifleBurst(int numBullets)
    {
        doneAttacking = false;
        var position = bossPart.position;
        Vector2 originalDir = (target.position - position);
        Vector2 dir = originalDir.normalized;
        
        for(int i = 0; i < numBullets; i++)
        {
            Attack(position, dir);
            yield return new WaitForSeconds(0.2f);
        }
        
        doneAttacking = true;
    }
}
