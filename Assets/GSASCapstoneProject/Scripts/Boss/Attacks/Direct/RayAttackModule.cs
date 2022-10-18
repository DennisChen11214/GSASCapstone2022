using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RayAttackModule : MonoBehaviour
{
    [SerializeField] private Ray _ray;
    private Ray[] _pool;
    [SerializeField] private int _poolSize;
    [SerializeField] private Transform _poolParent;

    private void Start()
    {
        // Instantiate attacks (projectiles) in the pool
        _pool = new Ray[_poolSize];
        if (_poolParent == null)
        {
            _poolParent = transform;
        }
        for(int i = 0; i < _poolSize; i++)
        {
            _pool[i] = Instantiate<Ray>(_ray, transform.position, Quaternion.identity, _poolParent);
            _pool[i].gameObject.SetActive(false);
        }
        
    }
    
    
    public void Attack(Vector2 targetPosition, Transform bossPart)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
            {
                _pool[i].Attach(bossPart);
                _pool[i].SetDirection(targetPosition - (Vector2)bossPart.position);
                _pool[i].Shoot();
                _pool[i].gameObject.SetActive(true);
                break;
            }
        }
    }
}
