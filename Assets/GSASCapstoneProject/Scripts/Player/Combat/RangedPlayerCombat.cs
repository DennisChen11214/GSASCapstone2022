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
    [SerializeField]
    float _timeBetweenAttacks;

    private SpriteRenderer _sprite;
    private Projectile[] _bulletPool;
    private float _prevAttackTIme;


    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _bulletPool = new Projectile[_poolSize];
        for(int i = 0; i < _poolSize; i++)
        {
            _bulletPool[i] = Instantiate<Projectile>(_bulletPrefab, _spawnPosition.position, Quaternion.identity, _poolParent);
        }
    }

    public override void Attack()
    {
        if (Time.time - _prevAttackTIme < _timeBetweenAttacks) return;
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_bulletPool[i].gameObject.activeSelf)
            {
                Vector3 bulletSpawn = _spawnPosition.position;
                _bulletPool[i].transform.position = bulletSpawn;
                _bulletPool[i].gameObject.SetActive(true);
                _bulletPool[i].GetComponent<Projectile>().Launch(Vector3.right);
                break;
            }
        }
        _prevAttackTIme = Time.time;
    }

    public override void TakeDamage(float damage)
    {
        _playerHealth.Value -= damage;
    }
}
