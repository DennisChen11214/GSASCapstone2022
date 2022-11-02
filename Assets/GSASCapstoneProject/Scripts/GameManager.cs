using Core.GlobalVariables;
using Core.GlobalEvents;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BoolVariable _player1Dead;
    [SerializeField]
    private BoolVariable _player2Dead;
    [SerializeField]
    private GlobalEvent _gameOver;

    private void Awake()
    {
        _player1Dead.Subscribe(CheckForLoss);
        _player2Dead.Subscribe(CheckForLoss);
    }

    private void CheckForLoss(bool temp)
    {
        if(_player1Dead.Value && _player2Dead.Value)
        {
            _gameOver.Raise();
        }
    }
}
