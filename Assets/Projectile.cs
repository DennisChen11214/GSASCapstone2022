using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float _speed;
    [SerializeField]
    float _damage;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector3 direction)
    {
        _rb.velocity = direction * _speed;
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
