using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedPlayerCombat : PlayerCombat
{
    [SerializeField]
    Projectile _bulletPrefab;
    [SerializeField]
    int _poolSize;
    [SerializeField]
    Transform _poolParent;
    [SerializeField]
    Transform _spawnPosition;

    private Projectile[] _bulletPool;
    private float _prevAttackTIme;

    protected override void Awake()
    {
        base.Awake();
        InitializeBulletPool();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void InitializeBulletPool()
    {
        _bulletPool = new Projectile[_poolSize];
        for (int i = 0; i < _poolSize; i++)
        {
            _bulletPool[i] = Instantiate<Projectile>(_bulletPrefab, _spawnPosition.position, Quaternion.identity, _poolParent);
        }
    }

    public override void Attack()
    {
        if (Time.time - _prevAttackTIme < _stats.TimeBeforeAttackResets) return;
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_bulletPool[i].gameObject.activeSelf)
            {
                Vector3 bulletSpawn = _spawnPosition.position;
                _bulletPool[i].transform.position = bulletSpawn;
                _bulletPool[i].gameObject.SetActive(true);
                _bulletPool[i].GetComponent<Projectile>().Launch(Vector3.right * (transform.localScale.x > 0 ? 1 : -1));
                break;
            }
        }
        _prevAttackTIme = Time.time;
    }
}