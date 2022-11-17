using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ray : MonoBehaviour
{
    // the time delay time for charging the ray attack (will be replaced with animation time)
    [SerializeField] private float delay;
    // the duration (in seconds) of the lightning remaining in the Arena
    [SerializeField] private float duration;
    
    [SerializeField] private Collider2D rayCollider;
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private bool shootLeft;
    [SerializeField] private Transform attachTarget;
    
    private void Start()
    {
        sprite.enabled = false;
        rayCollider.enabled = false;
        gameObject.SetActive(false);
    }

    
    private void Update()
    {
        transform.position = attachTarget.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  
        Vector2 collisionDir = collision.transform.position - transform.position;
        if (shootLeft && collisionDir.x >= 0 ||
            !shootLeft && collisionDir.x < 0)
        {
            return;
        }
        
        PlayerCombat playerCombat = collision.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            StopCoroutine(_Shoot());
            sprite.enabled = false;
            rayCollider.enabled = false;
            playerCombat.TakeDamage(transform.position.x <= collision.gameObject.transform.position.x);
            StartCoroutine(Fade());
        }
    }

    public void SetDirection(Vector2 rayDirection)
    {
        shootLeft = (rayDirection.x < 0);
    }
    
    public void Attach(Transform bossPart)
    {
        attachTarget = bossPart;
    }

    public void Shoot()
    {
        StartCoroutine(_Shoot());
    }
    
    private IEnumerator _Shoot()
    {
        // TODO: play the animation of charging
        
        yield return new WaitForSeconds(delay);    // replace this with wait until charging animation finished
        sprite.enabled = true;
        rayCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        rayCollider.enabled = false;
        sprite.enabled = false;
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
