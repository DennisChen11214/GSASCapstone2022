using System;
using System.Collections;
using System.Collections.Generic;
using Core.GlobalEvents;
using Core.GlobalVariables;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    // the time delay time(in seconds) for charging the lightning attack (will be replaced with animation time)
    [SerializeField] private float delay;
    // the duration (in seconds) of the lightning remaining in the Arena
    [SerializeField] private float duration;
    
    [SerializeField] private Collider2D lightningCollider;
    [SerializeField] private SpriteRenderer sprite;

    private void Start()
    {
        lightningCollider.enabled = false;
        sprite.enabled = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        PlayerCombat playerCombat = collision.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            StopCoroutine(_Strike());
            sprite.enabled = false;
            lightningCollider.enabled = false;
            playerCombat.TakeDamage(transform.position.x <= collision.gameObject.transform.position.x);
            StartCoroutine(Fade());
        }
    }
    

    public void Strike()
    {
        StartCoroutine(_Strike());
    }
    
    private IEnumerator _Strike()
    {
        // TODO: play the animation of charging
        
        yield return new WaitForSeconds(delay);    // replace this with wait until charging animation finished
        lightningCollider.enabled = true;
        sprite.enabled = true;
        yield return new WaitForSeconds(duration);
        lightningCollider.enabled = false;
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
