using System;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] string[] _ignoredTags;
    [SerializeField] float _speed = 20;
    [SerializeField] float _speedInc = 30;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCombat playerCombat = collision.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            playerCombat.TakeDamage();
        }
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _rb.velocity += _dir * (_speedInc * Time.deltaTime);
    }
}
