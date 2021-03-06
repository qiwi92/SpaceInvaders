﻿using System;
using System.Collections.Generic;
using Enemies;
using GameLogic;
using UnityEngine;
using Weapons.Bullet;
using UniRx;

namespace Player.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _maxMoveBound;
        [SerializeField] private float _movementSpeed;

        private float _mainWeaponCooldown;
        private float _mainWeaponCooldownTimer = 0;

        [SerializeField] private float _bulletSpeed;
        [SerializeField] private float _bulletMaxTravelDistance;
        [SerializeField] private BulletView _bulletPrefab;

        [SerializeField] private ParticleSystem _deathParticleSystem;
        [SerializeField] private ParticleSystem _coinParticleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private readonly List<BulletView> _bullets = new List<BulletView>();
        private readonly List<BulletView> _deadBullets = new List<BulletView>();

        private PlayerState _playerState = PlayerState.Spawning;
        private bool _isDead;
        private ParticleSystem.EmitParams _emitParams = new ParticleSystem.EmitParams { applyShapeToPosition = true };

        private int _firedBullets;
        private int _missedBullets;

        public float Accuracy => GetAccuary();

        public event Action PlayerIsDead;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void Start()
        {
            _disposables.Add(GameState.PlayerStats.WeaponCooldown.Subscribe(cd => _mainWeaponCooldown = cd));
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        void Update ()
        {
            switch (_playerState)
            {
                case PlayerState.Dead:
                    HandleDead();
                    break;
                case PlayerState.Spawning:
                    HandleSpawning();
                    break;
                case PlayerState.Alive:
                    HandleAlive();
                    break;
                case PlayerState.Dying:
                    HandleDying();
                    break;
            }
        }

        private void HandleDead()
        {
            DestroyDeadBullets();
        }

        private void HandleSpawning()
        {
            _spriteRenderer.enabled = true;
            _playerState = PlayerState.Alive;
        }

        private void HandleAlive()
        {
            HandleMovementInput();
            HandleShooting();
        }

        private void HandleDying()
        {
            _spriteRenderer.enabled = false;

            
            _emitParams.position = transform.position;
            _deathParticleSystem.Emit(_emitParams, 20);

            PlayerIsDead?.Invoke();
            _playerState = PlayerState.Dead; 

        }

        private void HandleShooting()
        {

        
            if (_mainWeaponCooldownTimer > 0)
            {
                _mainWeaponCooldownTimer -= Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Space) & _mainWeaponCooldownTimer <= 0)
            {
                _mainWeaponCooldownTimer = _mainWeaponCooldown;

                var numOfBullets = GameState.PlayerStats.BulletAmount.Value;

                if (numOfBullets == 1)
                {
                    _bullets.Add(Instantiate(_bulletPrefab, transform.position, Quaternion.identity));
                }
                else
                {
                    for (var i = 0; i < numOfBullets; i++)
                    {
                        var pos = transform.position + Vector3.right*0.2f*(i  - (numOfBullets-1)/2f);
                        _bullets.Add(Instantiate(_bulletPrefab, pos, Quaternion.identity));
                    }

                }

                
                _firedBullets += 1;
            }
            

            DestroyDeadBullets();


            foreach (var deadBullet in _deadBullets)
            {
                _bullets.Remove(deadBullet);
                Destroy(deadBullet.gameObject);
            }

            _deadBullets.Clear();
        }

        private void DestroyDeadBullets()
        {
            foreach (var bullet in _bullets)
            {
                if (bullet.transform.position.y > _bulletMaxTravelDistance )
                {
                    _missedBullets += 1;
                    bullet.IsDead = true;
                }

                if( bullet.IsDead)
                {
                    _deadBullets.Add(bullet);
                }

                bullet.transform.position += _bulletSpeed * Time.smoothDeltaTime * Vector3.up;
            }
        }

        private void HandleMovementInput()
        {
            var speedInput = 0;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                speedInput = -1;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                speedInput = 1;
            }

            if (Mathf.Abs(transform.position.x) < _maxMoveBound)
            {
                transform.position += _movementSpeed * speedInput * Time.smoothDeltaTime * Vector3.right;
            }
            else
            {
                if (transform.position.x > _maxMoveBound)
                {
                    if (speedInput < 0)
                    {
                        transform.position += _movementSpeed * speedInput * Time.smoothDeltaTime * Vector3.right;
                    }
                }
                else
                {
                    if (speedInput > 0)
                    {
                        transform.position += _movementSpeed * speedInput * Time.smoothDeltaTime * Vector3.right;
                    }
                }
            }
        }

        public void Die()
        {
            if (!_isDead)
            {
                _isDead = true;

                _playerState = PlayerState.Dying;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == 11)
            {
                if (_playerState == PlayerState.Alive)
                {
                    var bullet = other.GetComponent<IBullet>();

                    bullet.IsDead = true;

                    _playerState = PlayerState.Dying;
                }
            }

            if (other.gameObject.layer == 13)
            {
                if (_playerState == PlayerState.Alive)
                {
                    _playerState = PlayerState.Dying;
                }
            }

            if (other.gameObject.layer == 12)
            {
                if (_playerState == PlayerState.Alive)
                {
                    var coin = other.GetComponent<Coin>();
                    GameState.AddMoney(coin.Value);

                    _coinParticleSystem.Stop();
                    _coinParticleSystem.Emit(1);

                    coin.IsDead = true;
                }
            }
        }

        private float GetAccuary()
        {
            if (_firedBullets == 0)
            {
                return 1;
            }

            return _missedBullets / (float)_firedBullets;
        }
    }
}