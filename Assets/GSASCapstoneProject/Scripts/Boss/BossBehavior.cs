 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] FloatVariable hp;
    [SerializeField] private GlobalEvent onHealthBelowZero;
    [SerializeField] private GlobalEvent bossTakesDamage;
    [SerializeField] private SpriteRenderer sprite;

    private Color _c;
    
    
    private void Start()
    {
        onHealthBelowZero.Subscribe(Death);
        bossTakesDamage.Subscribe(ShowDamageEffect);
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        _c = sprite.color;
    }
    
    private void Death()
    {
        Destroy(gameObject);
    }

    private void ShowDamageEffect()
    {
        StartCoroutine(_DamageEffect());
    }
    
    IEnumerator _DamageEffect()
    {
        sprite.color = _c + new Color(1.0f, 0.1f, 0.0f, 0.3f);
        yield return new WaitForSeconds(0.2f);
        sprite.color = _c;
    }
}
