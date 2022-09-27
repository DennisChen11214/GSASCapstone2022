using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;
using Core.GlobalEvents;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] FloatVariable _hp;
    [SerializeField] private GlobalEvent _onHealthBelowZero;
    
    [SerializeField] TransformVariable[] _targets;
    private int _targetIdx = 0;
    [SerializeField] GlobalEvent _swap;
    
    [SerializeField] BossBullet _bullet;
    private BossBullet[] _bulletPool;
    [SerializeField] int _poolSize;
    [SerializeField] Transform _poolParent;

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

    private void BulletAttack(Vector3 dir)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_bulletPool[i].gameObject.activeSelf)
            {
                var position = transform.position;
                _bulletPool[i].transform.position = position;
                _bulletPool[i].gameObject.SetActive(true);
                _bulletPool[i].GetComponent<BossBullet>().Launch(dir);
                break;
            }
        }
    }
    
    IEnumerator StartBulletAttack()
    {
        while(true)
        {
            Vector2 dir = _targets[_targetIdx].Value.position - transform.position;
            dir = dir.normalized;
            BulletAttack(dir);
            yield return new WaitForSeconds(3);
        }
       
    }
    
    private void Start()
    {
        _swap.Subscribe(SwapTarget);
        _onHealthBelowZero.Subscribe(Death);
        
        _bulletPool = new BossBullet[_poolSize];
        for(int i = 0; i < _poolSize; i++)
        {
            _bulletPool[i] = Instantiate<BossBullet>(_bullet, transform.position, Quaternion.identity, _poolParent);
            _bulletPool[i].gameObject.SetActive(false);
        }
        StartCoroutine(StartBulletAttack());
    }

    private void SwapTarget()
    {
        _targetIdx = (_targetIdx + 1) % 2;
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
