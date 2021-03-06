﻿using System;
using System.Collections.Generic;
using Enemies;
using GameLogic;
using UniRx;
using UnityEngine;
using Weapons.Bullet;
using Random = UnityEngine.Random;

namespace Player.Controller
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyDamageController _damageController;
        [SerializeField] private ParticleSystem _deathParticleSystem;
        [SerializeField] private ParticleSystem _impactParticleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private EnemySprites _sprites;

        [SerializeField] private int _score;

        private EnemyState _enemyState = EnemyState.StartingSpawn;
        
        public bool IsDead { get; private set; }

        private readonly List<BulletView> _bullets = new List<BulletView>();
        private readonly List<BulletView> _deadBullets = new List<BulletView>();

        [SerializeField] private float _bulletSpeed;
        [SerializeField] private float _bulletMaxTravelDistance;
        [SerializeField] private BulletView _bulletPrefab;

        private float _weaponCooldown;
        private float _mainWeaponCooldownTimer;

        [SerializeField] private int _hitPoints;
        private int _hp = 1;
        private int _hpMax = 0;
        private ParticleSystem.EmitParams _emitParams;

        [SerializeField] private EnemyType _enemyType;
        private bool _canShoot;
        private int _coinValue;
        private bool _hasCoin;
        private Coin _coinPrefab;
        private ReactiveProperty<int> _coinAmount;
        private float _spawnDuration;
        private float _spawnTimer;

        [SerializeField] private ParticleSystem _spawnParticleSystem;

        public void Setup(ColorType color, float cooldown, int hp, int hpMulti)
        {
            _spriteRenderer.sprite = _sprites.GetSprite(color, _enemyType);
            _weaponCooldown = cooldown;

            _hitPoints += hp;
            _hitPoints *= hpMulti;

            _hpMax = _hitPoints;
        }

        void Start()
        {
            IsDead = false;

            _mainWeaponCooldownTimer = MainWeaponCooldownTimer();

            _emitParams = new ParticleSystem.EmitParams
            {
                applyShapeToPosition = true
            };


            switch (_enemyType)
            {
                case EnemyType.None:
                    break;
                case EnemyType.Regular:
                    _hp = _hitPoints;
                    _canShoot = false;
                    break;
                case EnemyType.Shooter:
                    _hp = _hitPoints;
                    _canShoot = true;
                    break;
                case EnemyType.Tank:
                    _hp = _hitPoints;
                    _canShoot = false;
                    break;
                case EnemyType.ShootingTank:
                    _hp = _hitPoints;
                    _canShoot = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        void Update()
        {
            switch (_enemyState)
            {
                case EnemyState.Dead:
                    HandleDead();
                    break;
                case EnemyState.StartingSpawn:
                    HandleStartSpawning();
                    break;
                case EnemyState.Spawning:
                    HandleSpawning();
                    break;
                case EnemyState.EndingSpawn:
                    HandleEndSpawning();
                    break;
                case EnemyState.Alive:
                    HandleAlive();
                    break;
                case EnemyState.Dying:
                    HandleDying();
                    break;
                
            }
        }

        

        
        private void HandleDead()
        {
            MoveBullets();
            //_enemyState = EnemyState.Spawning;
        }

        private void HandleStartSpawning()
        {
            _spawnDuration = Random.Range(0.1f, 1f);
            _spawnParticleSystem.Play();
            _spriteRenderer.enabled = false;


            _enemyState = EnemyState.Spawning;
            
        }

        private void HandleSpawning()
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer > _spawnDuration)
            {
                _enemyState = EnemyState.EndingSpawn;
            }
        }

        private void HandleEndSpawning()
        {
            _spawnParticleSystem.Stop();
            _spriteRenderer.enabled = true;

            _enemyState = EnemyState.Alive;
        }

        private void HandleAlive()
        {
            HandleShooting();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == 10)
            {
                if (_enemyState == EnemyState.Alive && _hp > 0)
                {
                    var bullet = other.GetComponent<BulletView>();

                    bullet.IsDead = true;

                    _hp -= 1;

                    _damageController.SetDamage(_hp/ (float) _hpMax);

                    if (_hp > 0)
                    {
                        _impactParticleSystem.Emit(_emitParams, 20);
                    }

                    if (_hp == 0)
                    {
                        
                        _enemyState = EnemyState.Dying;
                    }
                }
            }    
        }

        private void SpawnCoin()
        {
            var newCoin = Instantiate(_coinPrefab, transform.position, Quaternion.identity);
            newCoin.Amount = _coinAmount;
            newCoin.Value = _coinValue;
        }


        private void HandleDying()
        {
            _damageController.Disable();

            if (_hasCoin)
            {
                SpawnCoin();
            }

            GameState.AddScore(_score);

            IsDead = true;
            _spriteRenderer.enabled = false;

            _emitParams.position = transform.position;
            _deathParticleSystem.Emit(_emitParams, 40);
            _enemyState = EnemyState.Dead;
        }

        private void HandleShooting()
        {
            if (_canShoot)
            {
                if (_mainWeaponCooldownTimer > 0)
                {
                    _mainWeaponCooldownTimer -= Time.deltaTime;
                }

                if (_mainWeaponCooldownTimer <= 0)
                {
                    _mainWeaponCooldownTimer = MainWeaponCooldownTimer();
                    _bullets.Add(Instantiate(_bulletPrefab, transform.position, Quaternion.identity));
                }
            }
            

            MoveBullets();


            DestroyDeadBullets();

            _deadBullets.Clear();
        }

        private void MoveBullets()
        {
            foreach (var bullet in _bullets)
            {
                if (bullet.transform.position.y < -_bulletMaxTravelDistance || bullet.IsDead)
                {
                    _deadBullets.Add(bullet);
                }

                bullet.transform.position += _bulletSpeed * Time.smoothDeltaTime * Vector3.down;
            }
        }

        private void DestroyDeadBullets()
        {
            foreach (var deadBullet in _deadBullets)
            {
                _bullets.Remove(deadBullet);
                Destroy(deadBullet.gameObject);
            }
        }

        private float MainWeaponCooldownTimer()
        {
            return _weaponCooldown * Random.Range(0.1f,1f);
        }

        public void SetupCoin(int coinValue, Coin coinPrefab, ReactiveProperty<int> coinAmount)
        {
            coinAmount.Value += 1;
            _coinAmount = coinAmount;
            _coinValue = coinValue;
            _hasCoin = true;
            _coinPrefab = coinPrefab;
        }
    }
}