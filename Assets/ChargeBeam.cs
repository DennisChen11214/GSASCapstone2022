using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBeam : MonoBehaviour
{
    [SerializeField]
    private ScriptableStats _stats;
    [SerializeField]
    private float damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject);
        DamageModule damageModule = other.gameObject.GetComponent<DamageModule>();
        if (damageModule != null)
        {
            damageModule.TakeDamage(damage);
        }
    }
}
