///
/// Created by Dennis Chen
///

using Core.GlobalVariables;
using UnityEngine;

public class ChargeBeam : MonoBehaviour
{
    [SerializeField]
    private ScriptableStats _stats;
    [SerializeField]
    private FloatVariable _damageDealt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If this hits a boss, deal damage to it
        DamageModule damageModule = other.gameObject.GetComponent<DamageModule>();
        if (damageModule != null)
        {
            damageModule.TakeDamage(_stats.ChargeBeamDamage);
            _damageDealt.Value += _stats.ChargeBeamDamage;
        }
    }
}
