///
/// Created by Dennis Chen
///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalEvents;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    RectTransform _player1UI;
    [SerializeField]
    RectTransform _player2UI;
    [SerializeField]
    GlobalEvent _swapCompleted;

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

    private void OnEnable()
    {
        _swapCompleted.Subscribe(SwapUI);
    }

    private void OnDisable()
    {
        _swapCompleted.Unsubscribe(SwapUI);   
    }
}
