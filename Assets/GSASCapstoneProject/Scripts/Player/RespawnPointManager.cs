///
/// Created by Dennis Chen
///

using Core.GlobalEvents;
using UnityEngine;
using System.Collections.Generic;

public class RespawnPointManager : MonoBehaviour
{
    [SerializeField] private GlobalEvent _onPlayerDied;
    [SerializeField] private GlobalEvent _onPlayerRevived;
    [SerializeField] private GlobalEvent _onOtherPlayerRevived;
    [SerializeField] private GlobalEvent _onPlayerSwapped;
    [SerializeField] private GameObject _player;
    [SerializeField] private int _playerNum;

    private static List<Transform>[] _respawnPointsList = new List<Transform>[2];
    private List<Transform> _myRespawnPoints;

    private void Awake()
    {
        _respawnPointsList[_playerNum] = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++)
        {
            _respawnPointsList[_playerNum].Add(transform.GetChild(i));
        }
        _myRespawnPoints = _respawnPointsList[_playerNum];
    }

    private void PlayersSwapped()
    {
        _playerNum = (_playerNum + 1) % 2;
        _myRespawnPoints = _respawnPointsList[_playerNum];
    }

    private void PlayerDied()
    {
        int rand = Random.Range(0, _myRespawnPoints.Count);
        _myRespawnPoints[rand].gameObject.SetActive(true);
    }

    private void PlayerRevived()
    {
        int rand = Random.Range(0, _myRespawnPoints.Count);
        _player.transform.position = _myRespawnPoints[rand].position;
        _player.SetActive(true);
    }

    private void OtherPlayerRevived()
    {
        for (int i = 0; i < _myRespawnPoints.Count; i++)
        {
            GameObject child = _myRespawnPoints[i].gameObject;
            if (child.activeSelf)
            {
                child.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        _onPlayerDied.Subscribe(PlayerDied);
        _onPlayerRevived.Subscribe(PlayerRevived);
        _onOtherPlayerRevived.Subscribe(OtherPlayerRevived);
        _onPlayerSwapped.Subscribe(PlayersSwapped);
    }

    private void OnDisable()
    {
        _onPlayerDied.Unsubscribe(PlayerDied);
        _onPlayerRevived.Unsubscribe(PlayerRevived);
        _onOtherPlayerRevived.Unsubscribe(OtherPlayerRevived);
        _onPlayerSwapped.Unsubscribe(PlayersSwapped);
    }
}
