///
/// Created by Dennis Chen
///

using UnityEngine;
using Core.GlobalVariables;

public class RangedPlayerCombat : PlayerCombat
{
    [SerializeField]
    private BoolVariable _isCharging;
    [SerializeField]
    private Projectile _bulletPrefab;
    [SerializeField]
    private int _poolSize;
    [SerializeField]
    private Transform _poolParent;
    [SerializeField]
    private Transform _spawnPosition;

    private Projectile[] _bulletPool;
    private float _attackEndTime;
    private float _curCharge;
    private bool _hasAttackBuffered = false;
    private bool _isComboFinished = false;
    private int _currentAttack = 0;

    protected override void Awake()
    {
        base.Awake();
        InitializeBulletPool();
    }

    protected override void Update()
    {
        base.Update();
        if (_isCharging.Value)
        {
            _curCharge = Mathf.Min(_curCharge + Time.deltaTime, _stats.ChargeTime);
            GetComponent<SpriteRenderer>().color = Color.green;
            if (_curCharge >= _stats.ChargeTime)
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
        _isCharging.Value = true;
        _curCharge = 0;
        if (_isAttacking.Value)
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
        _isAttacking.Value = true;
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

    public override void IncreaseCharge(float percent)
    {
        _curCharge = Mathf.Clamp(_curCharge + _stats.ChargeTime * percent, 0, _stats.ChargeTime);
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
        _isAttacking.Value = false;
        _currentAttack = (_currentAttack + 1) % 3;
        if(_currentAttack == 0)
        {
            _isComboFinished = true;
        }
        CheckForBufferedAttack();
    }

    public override void CancelAttack()
    {
        if(_curCharge >= _stats.ChargeTime)
        {
            _anim.SetTrigger("ChargeBeam");
        }
        _isCharging.Value = false;
        _curCharge = 0;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void CancelIfKnockedBack(bool isKnockedBack)
    {
        if (isKnockedBack)
        {
            _isCharging.Value = false;
            _curCharge = 0;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void CheckForBufferedAttack()
    {
        if (_hasAttackBuffered)
        {
            _isAttacking.Value = true;
            _hasAttackBuffered = false;
            switch (_currentAttack)
            {
                case 0:
                    _isAttacking.Value = false;
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
        _isAttacking.Value = false;
        _isKnockedBack.Subscribe(CancelIfKnockedBack);
    }

    private void OnDisable()
    {
        _isKnockedBack.Unsubscribe(CancelIfKnockedBack);
    }
}
