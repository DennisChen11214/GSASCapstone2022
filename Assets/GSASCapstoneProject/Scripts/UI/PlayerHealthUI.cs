///
/// Created by Dennis Chen
///

using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField]
    private IntVariable _playerHealth;
    [SerializeField]
    private GameObject _heart;

    private List<GameObject> _hearts;

    private void Start()
    {
        _hearts = new List<GameObject>();
        float _heartSize = _heart.GetComponent<RectTransform>().rect.width * 0.9f;
        for(int i = 0; i < _playerHealth.Value; i++)
        {
            Vector3 position = new Vector3(transform.position.x + _heartSize * i, transform.position.y, transform.position.z);
            _hearts.Add(Instantiate(_heart, position, Quaternion.identity, transform));
        }
    }

    //Update the number of hearts according to the player health
    private void UpdateHearts(int health)
    {
        if (_hearts == null || _hearts.Count == 0) return;
        for(int i = 0; i < health; i++)
        {
            _hearts[i].SetActive(true);
        }
        for(int i = health; i < _hearts.Count && i >= 0; i++)
        {
            _hearts[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        _playerHealth.Subscribe(UpdateHearts);
    }

    private void OnDisable()
    {
        _playerHealth.Unsubscribe(UpdateHearts);
    }
}
