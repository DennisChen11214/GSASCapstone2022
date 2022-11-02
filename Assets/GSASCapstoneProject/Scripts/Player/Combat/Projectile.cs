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
        if(Time.time - curTime > _stats.ProjectileDuration)
        {
            gameObject.SetActive(false);
        }
    }

    public void Launch(Vector2 direction)
    {
        _rb.velocity = direction * _stats.ProjectileSpeed;
        curTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
