using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float _speed;
    [SerializeField]
    float _damage;
    [SerializeField]
    float _duration;

    private float curTime;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Time.time - curTime > _duration)
        {
            gameObject.SetActive(false);
        }
    }

    public void Launch(Vector2 direction)
    {
        _rb.velocity = direction * _speed;
        curTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageModule damageModule = collision.GetComponent<DamageModule>();
        if (!damageModule)
        {
            gameObject.SetActive(false);
            return;
        }
        damageModule.TakeDamage(_damage);
        gameObject.SetActive(false);
    }
}
