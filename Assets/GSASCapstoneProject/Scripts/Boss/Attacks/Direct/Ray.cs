///
/// Created by Dennis Chen
///

using System.Collections;
using UnityEngine;

public class Ray : MonoBehaviour
{
    // the time delay time for charging the ray attack 
    [SerializeField] private float _chargeDelay;

    // the time delay time between charging and shooting the ray attack 
    [SerializeField] private float _shootDelay;

    // the duration (in seconds) of the laser staying after firing
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
        //Follow the target if this is currently charging
        if (_charging)
        {
            transform.right = _target.position - transform.position;
            _delay -= Time.deltaTime;
            //Start the shoot process once we're done charging
            if(_delay < 0)
            {
                _charging = false;
                Shoot();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  
        //Deal damage to the player if it hit
        PlayerCombat playerCombat = collision.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            playerCombat.TakeDamage(transform.position.x <= collision.gameObject.transform.position.x);
        }
    }
    
    //Sets up the variables to start shooting at a given target
    public void StartShooting(Transform target)
    {
        _target = target;
        transform.right = _target.position - transform.position;
        //Have the sprite be more transparent to indicate that it's in the first stage
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.33f);
        _charging = true;
        _delay = _chargeDelay;
    }

    public void Shoot()
    {
        StartCoroutine(_Shoot());
    }
    
    //After the ray is locked on, wait a bit and then fire
    private IEnumerator _Shoot()
    {
        //Have the sprite be a bit transparent to indicate that it's in the second stage
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.66f);
        yield return new WaitForSeconds(_shootDelay);  
        _rayCollider.enabled = true;
        //Have the sprite be a opaque to indicate that it has fired
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1f);
        yield return new WaitForSeconds(_duration);
        _rayCollider.enabled = false;
        gameObject.SetActive(false);
        yield return null;
    }
    
}
