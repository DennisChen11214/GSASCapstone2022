using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

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
    private float _attackEndTime;
    private float _curCharge;
    private bool _hasAttackBuffered = false;
    private bool _isAttacking = false;
    private bool _isComboFinished = false;
    private bool _charging = false;
    private int _currentAttack = 0;

    protected override void Awake()
    {
        base.Awake();
        InitializeBulletPool();
    }

    protected override void Update()
    {
        base.Update();
        if (_charging)
        {
            _curCharge += Time.deltaTime;
            if(_curCharge > _stats.ChargeTime)
            {
                GetComponent<SpriteRenderer>().color = Color.black;
            }  
        }
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
        _charging = true;
        if (_isAttacking)
        {
            _hasAttackBuffered = true;
            return;
        }
        if(_isComboFinished && Time.time - _attackEndTime < _stats.TimeBeforeNextCombo)
        {
            return;
        }
        _isComboFinished = false;
        if (Time.time - _attackEndTime > _stats.TimeBeforeAttackResets)
        {
            _currentAttack = 0;
        }
        _isAttacking = true;
        switch (_currentAttack)
        {
            case 0:
                _anim.SetTrigger("Attack1");
                break;
            case 1:
                _anim.SetTrigger("Attack2");
                break;
            case 2:
                _anim.SetTrigger("Attack3");
                break;
        }
    }

    public void SpawnProjectiles()
    {
        int numSpawned = 0;
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_bulletPool[i].gameObject.activeSelf)
            {
                Vector3 bulletSpawn = _spawnPosition.position;
                _bulletPool[i].transform.position = bulletSpawn;
                _bulletPool[i].gameObject.SetActive(true);
                float angle = Mathf.Deg2Rad * Random.Range(-_stats.AngleOfVariation, _stats.AngleOfVariation);
                Vector2 projDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
                _bulletPool[i].GetComponent<Projectile>().Launch(projDirection * (transform.localScale.x > 0 ? 1 : -1));
                numSpawned++;
                if (numSpawned == _stats.NumProjectilesAttack1) break;
            }
        }
    }

    public void OnAttackEnd()
    {
        _attackEndTime = Time.time;
        _isAttacking = false;
        _currentAttack = (_currentAttack + 1) % 3;
        if(_currentAttack == 0)
        {
            _isComboFinished = true;
        }
        CheckForBufferedAttack();
    }

    public override void CancelAttack(bool isAttackCanceled)
    {
        if (isAttackCanceled)
        {
            if(_curCharge > _stats.ChargeTime)
            {
                _anim.SetTrigger("ChargeBeam");
            }
            _charging = false;
            _curCharge = 0;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void CheckForBufferedAttack()
    {
        if (_hasAttackBuffered)
        {
            _isAttacking = true;
            _hasAttackBuffered = false;
            switch (_currentAttack)
            {
                case 0:
                    _isAttacking = false;
                    break;
                case 1:
                    _anim.SetTrigger("Attack2");
                    break;
                case 2:
                    _anim.SetTrigger("Attack3");
                    break;
            }
        }
    }

    private void OnEnable()
    {
        _hasAttackBuffered = false;
        _isAttacking = false;
        _isKnockedBack.Subscribe(CancelAttack);
    }

    private void OnDisable()
    {
        _isKnockedBack.Unsubscribe(CancelAttack);
    }
}
