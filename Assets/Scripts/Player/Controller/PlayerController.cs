using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Weapons.Bullet;

namespace Player.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _maxMoveBound;
        [SerializeField] private float _movementSpeed;

        [SerializeField] private float _mainWeaponCooldown;
        private float _mainWeaponCooldownTimer = 0;

        [SerializeField] private float _bulletSpeed;
        [SerializeField] private float _bulletMaxTravelDistance;
        [SerializeField] private BulletView _bulletPrefab;

        [SerializeField] private ParticleSystem _deathParticleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private readonly List<BulletView> _bullets = new List<BulletView>();
        private readonly List<BulletView> _deadBullets = new List<BulletView>();

        private PlayerState _playerState = PlayerState.Spawning;
        private bool _isDead;

        public event Action PlayerIsDead;

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

            var emitParams = new ParticleSystem.EmitParams
            {
                position = transform.position,
                applyShapeToPosition = true
            };
            _deathParticleSystem.Emit(emitParams, 20);

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
                _bullets.Add(Instantiate(_bulletPrefab, transform.position, Quaternion.identity ));
            }

            foreach (var bullet in _bullets)
            {
                if (bullet.transform.position.y > _bulletMaxTravelDistance || bullet.IsDead)
                {
                    _deadBullets.Add(bullet);
                }

                bullet.transform.position += _bulletSpeed * Time.smoothDeltaTime * Vector3.up;
            }


            foreach (var deadBullet in _deadBullets)
            {
                _bullets.Remove(deadBullet);
                Destroy(deadBullet.gameObject);
            }

            _deadBullets.Clear();
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
    }
}