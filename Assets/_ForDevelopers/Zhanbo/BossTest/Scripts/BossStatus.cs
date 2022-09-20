using System;
using System.Collections;
using System.Collections.Generic;
using Core.GlobalVariables;
using UnityEngine;
using UnityEngine.Events;
public class BossStatus : MonoBehaviour
{
    public FloatVariable hp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();
        if (dd != null)
        {
            hp.Value -= dd.damage;
            Debug.Log("Boss got hit");
        }
    }
}
