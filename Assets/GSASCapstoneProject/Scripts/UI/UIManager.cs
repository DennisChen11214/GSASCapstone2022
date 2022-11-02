///
/// Created by Dennis Chen
///

using TMPro;
using UnityEngine;
using Core.GlobalEvents;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _player1UI;
    [SerializeField]
    private RectTransform _player2UI;
    [SerializeField]
    private GameObject _endScreen;
    [SerializeField]
    private TMP_Text _endText;
    [SerializeField]
    private GlobalEvent _swapCompleted;
    [SerializeField]
    private GlobalEvent _playerWin;
    [SerializeField]
    private GlobalEvent _gameOver;

    private void SwapUI()
    {
        Vector2 tempMin = _player1UI.anchorMin;
        Vector2 tempMax = _player1UI.anchorMax;
        Vector2 tempPos = _player1UI.anchoredPosition;
        _player1UI.anchorMin = _player2UI.anchorMin;
        _player1UI.anchorMax = _player2UI.anchorMax;
        _player1UI.anchoredPosition = _player2UI.anchoredPosition;
        _player2UI.anchorMin = tempMin;
        _player2UI.anchorMax = tempMax;
        _player2UI.anchoredPosition = tempPos;
    }

    private void PlayerWin()
    {
        _endScreen.SetActive(true);
        _endText.text = "Victory";
    }

    private void GameOver()
    {
        _endScreen.SetActive(true);
        _endText.text = "Game Over";
    }

    private void OnEnable()
    {
        _swapCompleted.Subscribe(SwapUI);
        _playerWin.Subscribe(PlayerWin);
        _gameOver.Subscribe(GameOver);
    }

    private void OnDisable()
    {
        _swapCompleted.Unsubscribe(SwapUI);
        _playerWin.Unsubscribe(PlayerWin);
        _gameOver.Unsubscribe(GameOver);
    }
}
