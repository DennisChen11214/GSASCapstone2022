///
/// Created by Dennis Chen
///

using UnityEngine;
using Core.GlobalVariables;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _damageDealt;
    [SerializeField]
    private ScriptableStats _stats;

    private float curTime;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        //The projectile is set inactive if it's been active for a certain amount of time
        if(Time.time - curTime > _stats.ProjectileDuration)
        {
            gameObject.SetActive(false);
        }
    }

    //Sets the velocity of the projectile
    public void Launch(Vector2 direction)
    {
        _rb.velocity = direction * _stats.ProjectileSpeed;
        curTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if what this hit is the boss, and if so, deal damage to it
        DamageModule damageModule = collision.GetComponent<DamageModule>();
        if (!damageModule)
        {
            gameObject.SetActive(false);
            return;
        }
        damageModule.TakeDamage(_stats.ProjectileDamage);
        _damageDealt.Value += _stats.ProjectileDamage;
        gameObject.SetActive(false);
    }
}
