using UnityEngine;
using Core.GlobalVariables;

public class WallAttackModule : MonoBehaviour
{
    [SerializeField]
    private GameObject _thinWalls;
    [SerializeField]
    private GameObject _thickWall;

    private Vector3 _thinWallOriginalPos;
    private Vector3 _thickWallOriginalPos;


    private void Start()
    {
        _thinWallOriginalPos = _thinWalls.transform.position;
        _thickWallOriginalPos = _thickWall.transform.position;
    }

    public void StartAttack()
    {
        float rand = Random.Range(0, 1);
        if(rand <= 0.5f)
        {
            StartAttack(true);
        }
        else
        {
            StartAttack(false);
        }
    }

    public void StartAttack(bool thin)
    {
        if (thin)
        {
            _thinWalls.transform.position = _thinWallOriginalPos;
            _thinWalls.SetActive(true);
        }
        else
        {
            _thickWall.transform.position = _thickWallOriginalPos;
            _thickWall.SetActive(true);
        }
    }
}
