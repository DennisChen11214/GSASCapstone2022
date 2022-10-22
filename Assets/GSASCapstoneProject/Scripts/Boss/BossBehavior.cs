 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] FloatVariable _hp;
    [SerializeField] private GlobalEvent _onHealthBelowZero;


    // Player attacks must use the component "DamageDealer" to deal damage.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();
        if (dd != null && dd.CompareTag("PlayerAttack"))
        {
            _hp.Value -= dd.damage;
            Debug.Log("Boss got hit");
        }
    }

  
    
    
    private void Start()
    {
        _onHealthBelowZero.Subscribe(Death);
    }
    
    private void Death()
    {
        Destroy(gameObject);
    }
}
