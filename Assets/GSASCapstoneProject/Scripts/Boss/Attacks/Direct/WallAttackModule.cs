using UnityEngine;
using System.Collections.Generic;

public class WallAttackModule : MonoBehaviour
{
    [SerializeField]
    private Transform _thinWallsPoolParent;
    [SerializeField]
    private Transform _thickWallPoolParent;
    [SerializeField]
    private int _poolSize;
    [SerializeField]
    private GameObject _thinWall;
    [SerializeField]
    private GameObject _thickWall;

    private Vector3 _thinWallOriginalPos;
    private Vector3 _thickWallOriginalPos;

    private GameObject[] _thinWallsPool;
    private GameObject[] _thickWallPool;


    private void Start()
    {
        _thinWallOriginalPos = _thinWallsPoolParent.position;
        _thickWallOriginalPos = _thickWallPoolParent.position;
        InitializePool();
    }

    //Initializes a pool of thin and thick wall attacks
    private void InitializePool()
    {
        _thinWallsPool = new GameObject[_poolSize];
        for (int i = 0; i < _poolSize; i++)
        {
            _thinWallsPool[i] = Instantiate(_thinWall, Vector3.zero, Quaternion.identity, _thinWallsPoolParent);
        }

        _thickWallPool = new GameObject[_poolSize];
        for (int i = 0; i < _poolSize; i++)
        {
            _thickWallPool[i] = Instantiate(_thickWall, Vector3.zero, Quaternion.identity, _thickWallPoolParent);
        }
    }

    //Sets active one of the wall attacks in the thin or thick wall pools
    public void StartAttack(bool thin, bool enraged = false)
    {
        if (thin)
        {
            for(int i = 0; i < _thinWallsPool.Length; i++)
            {
                if (!_thinWallsPool[i].gameObject.activeSelf)
                {
                    _thinWallsPool[i].transform.position = _thinWallOriginalPos;
                    _thinWallsPool[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < _thickWallPool.Length; i++)
            {
                if (!_thickWallPool[i].gameObject.activeSelf)
                {
                    _thickWallPool[i].transform.position = _thickWallOriginalPos;
                    _thickWallPool[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
    }
}
