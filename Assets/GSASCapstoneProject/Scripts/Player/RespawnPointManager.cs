using Core.GlobalEvents;
using UnityEngine;
using System.Collections.Generic;

public class RespawnPointManager : MonoBehaviour
{
    [SerializeField] private GlobalEvent _onPlayerDied;
    [SerializeField] private GlobalEvent _onPlayerRevived;
    [SerializeField] private GlobalEvent _onOtherPlayerRevived;
    [SerializeField] private GameObject _player;
    private List<Transform> _respawnPoints;

    private void Awake()
    {
        _onPlayerDied.Subscribe(PlayerDied);
        _onPlayerRevived.Subscribe(PlayerRevived);
        _onOtherPlayerRevived.Subscribe(OtherPlayerRevived);
        _respawnPoints = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++)
        {
            _respawnPoints.Add(transform.GetChild(i));
        }
    }

    private void PlayerDied()
    {
        int rand = Random.Range(0, _respawnPoints.Count);
        _respawnPoints[rand].gameObject.SetActive(true);
    }

    private void PlayerRevived()
    {
        int rand = Random.Range(0, _respawnPoints.Count);
        _player.transform.position = _respawnPoints[rand].position;
        _player.SetActive(true);
    }

    private void OtherPlayerRevived()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.activeSelf)
            {
                child.SetActive(false);
            }
        }
    }
}
