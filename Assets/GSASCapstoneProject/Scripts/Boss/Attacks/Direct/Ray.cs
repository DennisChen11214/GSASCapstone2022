using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ray : MonoBehaviour
{
    // the time delay time for charging the ray attack (will be replaced with animation time)
    [SerializeField] private float _chargeDelay;

    // the time delay time between charging and shooting the ray attack (will be replaced with animation time)
    [SerializeField] private float _shootDelay;

    // the duration (in seconds) of the laser staying after firing(will be replaced with animation time)
    [SerializeField] private float _duration;
    
    [SerializeField] private Collider2D _rayCollider;
    
    private Transform _target;
    private SpriteRenderer _sprite;
    private bool _charging;
    private float _delay;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _rayCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (_charging)
        {
            transform.right = _target.position - transform.position;
            _delay -= Time.deltaTime;
            if(_delay < 0)
            {
                _charging = false;
                Shoot();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  
        PlayerCombat playerCombat = collision.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            playerCombat.TakeDamage(transform.position.x <= collision.gameObject.transform.position.x);
        }
    }
    
    public void StartShooting(Transform target)
    {
        _target = target;
        transform.right = _target.position - transform.position;
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.33f);
        _charging = true;
        _delay = _chargeDelay;
    }

    public void Shoot()
    {
        StartCoroutine(_Shoot());
    }
    
    private IEnumerator _Shoot()
    {
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.66f);
        yield return new WaitForSeconds(_shootDelay);    // replace this with wait until charging animation finished
        _rayCollider.enabled = true;
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1f);
        yield return new WaitForSeconds(_duration);
        _rayCollider.enabled = false;
        StartCoroutine(Fade());
        yield return null;
    }

    private IEnumerator Fade()
    {
        // TODO: play the animation of fading
        // yield return new WaitUntil(fading animation finished)
        gameObject.SetActive(false);
        yield return null;
    }
    
    
}
