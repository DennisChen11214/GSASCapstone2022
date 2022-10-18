using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletAttackModule : MonoBehaviour
{
    [SerializeField] private Bullet _bullet;
    private Bullet[] _pool;
    [SerializeField] private int _poolSize;
    [SerializeField] private Transform _poolParent;

    [SerializeField] private Transform target;
    [SerializeField] private Transform bossPart;
    
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

    public void Burst()
    {
        if (doneAttacking)
        {
            StartCoroutine(_Burst());
        }
    }
    
    private IEnumerator _Burst()
    {
        doneAttacking = false;
        var position = bossPart.position;
        Vector2 dir = (target.position - position);
        Vector2 dir2 = dir + Vector2.up;
        Vector2 dir3 = dir + Vector2.down;
        
        Attack(position, dir.normalized);
        yield return new WaitForSeconds(0.1f);
        
        Attack(position, dir2.normalized);
        yield return new WaitForSeconds(0.1f);
        
        Attack(position, dir3.normalized);
        yield return new WaitForSeconds(0.1f);
        
        doneAttacking = true;
    }
}
