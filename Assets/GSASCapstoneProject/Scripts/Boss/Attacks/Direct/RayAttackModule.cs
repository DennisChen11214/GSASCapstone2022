using Core.GlobalVariables;
using System.Collections.Generic;
using UnityEngine;

public class RayAttackModule : MonoBehaviour
{
    [SerializeField] private Ray _ray;
    [SerializeField] private int _poolSize;
    [SerializeField] private Transform _poolParent;
    [SerializeField] private List<Transform> _initialTransforms;
    [SerializeField] private BoolVariable _isBossEnraged;
    [SerializeField] private FloatVariable _enragedRayMultiplier;

    private Ray[] _pool;
    private List<Transform> _usedTransforms;

    private void Start()
    {
        // Instantiate attacks (projectiles) in the pool
        _pool = new Ray[_poolSize];
        for(int i = 0; i < _poolSize; i++)
        {
            _pool[i] = Instantiate<Ray>(_ray, transform.position, Quaternion.identity, _poolParent);
            _pool[i].gameObject.SetActive(false);
        }
        _usedTransforms = new List<Transform>();
    }
    
    
    public void Attack(Transform target, int numRays)
    {
        if (_isBossEnraged.Value)
        {
            numRays = Mathf.CeilToInt(numRays * _enragedRayMultiplier.Value);
        }
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
            {
                Transform randTransform = GetRandomTransform();
                while (_usedTransforms.Contains(randTransform))
                {
                    randTransform = GetRandomTransform();
                }
                _usedTransforms.Add(randTransform);
                _pool[i].transform.position = randTransform.position;
                _pool[i].gameObject.SetActive(true);
                _pool[i].StartShooting(target);
                numRays--;
            }
            if (numRays == 0)
            {
                _usedTransforms.Clear();   
                return;
            }
        }
    }

    private Transform GetRandomTransform()
    {
        int rand = Random.Range(0, _initialTransforms.Count);
        return _initialTransforms[rand];
    }
}
