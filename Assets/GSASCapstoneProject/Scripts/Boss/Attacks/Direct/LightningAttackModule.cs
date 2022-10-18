using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAttackModule : MonoBehaviour
{
    [SerializeField] private Lightning _lightning;
    [SerializeField] private Transform _ceiling;
    
    private Lightning[] _pool;
    [SerializeField] private int _poolSize;
    [SerializeField] private Transform _poolParent;
    
    private void Start()
    {
        // Instantiate attacks (lightning) in the pool
        _pool = new Lightning[_poolSize];
        if (_poolParent == null)
        {
            _poolParent = transform;
        }
        for(int i = 0; i < _poolSize; i++)
        {
            _pool[i] = Instantiate<Lightning>(_lightning, transform.position, Quaternion.identity, _poolParent);
            _pool[i].gameObject.SetActive(false);
        }
        
    }
    
    public void Attack(float posX)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
            {
                _pool[i].transform.position= new Vector2(posX, _ceiling.position.y);
                _pool[i].gameObject.SetActive(true);
                _pool[i].GetComponent<Lightning>().Strike();
                break;
            }
        }
    }
}
