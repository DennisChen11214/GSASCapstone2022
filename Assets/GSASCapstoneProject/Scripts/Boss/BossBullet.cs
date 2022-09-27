using System;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] string[] _ignoredTags;
    [SerializeField] float _speed = 20;
    [SerializeField] float _speedInc = 30;
    [SerializeField] float _damage = 10;
    private Vector2 _dir;
    private Rigidbody2D _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 dir)
    {
        _dir = dir;
        _rb.velocity = dir * _speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        for (int i = 0; i < _ignoredTags.Length; i++)
        {
            if (col.CompareTag(_ignoredTags[i])) return;
        }
        
        PlayerCombat playerCombat = col.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            playerCombat.TakeDamage(_damage);
        }
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _rb.velocity += _dir * (_speedInc * Time.deltaTime);
    }
}
