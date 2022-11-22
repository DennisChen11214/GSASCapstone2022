using UnityEngine;
using System.Collections.Generic;

public class WallAttackModule : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _thinWallsPool;
    [SerializeField]
    private List<GameObject> _thickWallPool;

    private Vector3 _thinWallOriginalPos;
    private Vector3 _thickWallOriginalPos;


    private void Start()
    {
        _thinWallOriginalPos = _thinWallsPool[0].transform.position;
        _thickWallOriginalPos = _thickWallPool[0].transform.position;
    }

    public void StartAttack(bool thin, bool enraged = false)
    {
        if (thin)
        {
            for(int i = 0; i < _thinWallsPool.Count; i++)
            {
                if (!_thinWallsPool[i].activeSelf)
                {
                    _thinWallsPool[i].transform.position = _thinWallOriginalPos;
                    _thinWallsPool[i].SetActive(true);
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < _thickWallPool.Count; i++)
            {
                if (!_thickWallPool[i].activeSelf)
                {
                    _thickWallPool[i].transform.position = _thickWallOriginalPos;
                    _thickWallPool[i].SetActive(true);
                    return;
                }
            }
        }
    }
}
