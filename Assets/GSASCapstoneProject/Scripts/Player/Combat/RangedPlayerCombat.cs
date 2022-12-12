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
    [SerializeField]
    private ParticleSystem _chargeParticles;
    [SerializeField]
    private GameObject _chargeBeam;

    private PlayerController _controller;
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
        _isCharging.Value = false;
        _controller = GetComponent<PlayerController>();
    }

    protected override void Update()
    {
        base.Update();
        if (_isCharging.Value)
        {
            _curCharge = Mathf.Min(_curCharge + Time.deltaTime, _stats.ChargeTime);
            //Change the particle effect once we're at max charge
            if (_curCharge >= _stats.ChargeTime)
            {
                ParticleSystem.MainModule main = _chargeParticles.main;
                ParticleSystem.ShapeModule shape = _chargeParticles.shape;
                main.startLifetime = 0.1f;
                main.startSize = 1.5f;
                shape.radius = 0.1f;
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
        //If we're already attacking, buffer this attack to start once the current one ends
        if (_isAttacking.Value)
        {
            _hasAttackBuffered = true;
            return;
        }
        //If we're in the downtime period between attack combos
        if(_isComboFinished && Time.time - _attackEndTime < _stats.TimeBeforeNextCombo)
        {
            return;
        }
        _isComboFinished = false;
        //If enough time has passed since our last enough, reset the attack to the first one
        if (Time.time - _attackEndTime > _stats.TimeBeforeAttackResets)
        {
            _currentAttack = 0;
        }
        _isAttacking.Value = true;
        _audio.PlayOneShot(_attackSound);
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

    //Updates particles if we're charging or stops them if we're not
    private void StartOrStopCharging(bool _isCharging)
    {
        if (_isCharging)
        {
            ParticleSystem.MainModule main = _chargeParticles.main;
            ParticleSystem.ShapeModule shape = _chargeParticles.shape;
            _chargeParticles.Play();
            main.startLifetime = 0.4f;
            main.startSize = 0.7f;
            shape.radius = 1.4f;
        }
        else
        {
            _chargeParticles.Stop();
        }
    }

    //Spawns the projectiles used for our attack and fires them in a random direction within a cone
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

    //Update the attack variables at the end of an attack animation
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

    //Occurs when the player lets go of the attack button
    //Checks if we've charged long enough for the beam to go off
    public override void CancelAttack()
    {
        if(_curCharge >= _stats.ChargeTime)
        {
            _audio.PlayOneShot(_attackSound);
            _anim.SetTrigger("ChargeBeam");
        }
        _isCharging.Value = false;
        _curCharge = 0;
    }

    //If the player gets hit, charging is canceled
    private void CancelIfKnockedBack(bool isKnockedBack)
    {
        if (isKnockedBack)
        {
            _isCharging.Value = false;
            _curCharge = 0;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    //At the end of an attack, we check if there's currently another one buffered. If so, then initiate that attack
    private void CheckForBufferedAttack()
    {
        if (_hasAttackBuffered)
        {
            _isAttacking.Value = true;
            _hasAttackBuffered = false;
            switch (_currentAttack)
            {
                //We don't attack again if we're at the end of the combo
                case 0:
                    _isAttacking.Value = false;
                    break;
                case 1:
                    _audio.PlayOneShot(_attackSound);
                    _anim.SetTrigger("Attack2");
                    break;
                case 2:
                    _audio.PlayOneShot(_attackSound);
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
        _isCharging.Subscribe(StartOrStopCharging);
    }

    private void OnDisable()
    {
        _isKnockedBack.Unsubscribe(CancelIfKnockedBack);
        _isCharging.Unsubscribe(StartOrStopCharging);
        _chargeBeam.SetActive(false);
        _isCharging.Value = false;
        _controller.IsShootingLaser = false;
    }
}
