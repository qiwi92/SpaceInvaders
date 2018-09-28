using System;
using System.Collections.Generic;
using Enemies;
using GameLogic;
using UnityEngine;
using Weapons.Bullet;
using Random = UnityEngine.Random;

namespace Player.Controller
{
    public class BossController : MonoBehaviour
    {
        public event Action BossDied;
        public event Action<float> PercentageHp;

        [SerializeField] private EnemyDamageController _damageController;
        [SerializeField] private ParticleSystem _deathParticleSystem;
        [SerializeField] private ParticleSystem _impactParticleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private int _score;

        private BossState _state = BossState.Spawning;

        public bool IsDead { get; private set; }

        private readonly List<BossBullet> _bullets = new List<BossBullet>();
        private readonly List<BossBullet> _deadBullets = new List<BossBullet>();

        [SerializeField] private BossType _bossType;

        private float _bulletMaxTravelDistance = 12;
        [SerializeField] private BossBullet _normalBulletPrefab;
        [SerializeField] private BossBullet _circleBulletPrefab;

        [SerializeField] private int _maxHitPoints;
        private int _hp;

        private ParticleSystem.EmitParams _emitParams;

        [SerializeField] private bool _hasNormalAttacks;
        [SerializeField] private float _normalBulletSpeed;
        [SerializeField] private float _normalAttackCooldown;
        [SerializeField] private int _normalAttackCount;
        [SerializeField] private float _normalAttacksSpacing;

        [SerializeField] private bool _hasCircleAttacks;
        [SerializeField] private float _circleBulletSpeed;
        [SerializeField] private float _circleAttackCooldown;
        [SerializeField] private int _circleAttackCount;

        [SerializeField] private bool _hasLaserAttacks;
        [SerializeField] private float _laserRotationSpeed;
        [SerializeField] private float _laserAttackCooldown;
        [SerializeField] private Transform _laserTransform;


        [SerializeField] private float _maxMoveRange = 3;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _verticalPosition;

        [SerializeField] private float _phaseOneWaitTime;
        [SerializeField] private float _phaseTwoWaitTime;
        
        private float _timer = 0;
        
        private float _normalAttackTimer = 0;
        private float _circleAttackTimer = 0;
        private float _laserAttackTimer = 0;

        private int _dir = 1;
        private int _laserDir = -1;
        private bool _laserCd;

        void Start()
        {
            _laserTransform.localRotation = Quaternion.Euler(0, 0, 0);
            _laserTransform.gameObject.SetActive(false);

            transform.position = Vector3.up * 10f;

            

            IsDead = false;

            _emitParams = new ParticleSystem.EmitParams
            {
                applyShapeToPosition = true
            };
            _hp = _maxHitPoints;
        }

        void Update()
        {
            switch (_state)
            {
                case BossState.Dead:
                    HandleDead();
                    break;
                case BossState.Spawning:
                    HandleSpawning();
                    break;
                case BossState.StartingPhaseOne:
                    HandleStartOfPhaseOne();
                    break;
                case BossState.PhaseOne:
                    HandlePhaseOne();
                    break;
                case BossState.StartingPhaseTwo:
                    HandleStartOfPhaseTwo();
                    break;
                case BossState.PhaseTwo:
                    HandlePhaseTwo();
                    break;
                case BossState.Dying:
                    HandleDying();
                    break;
            }
        }

        

        

        private void HandleDead()
        {
            MoveBullets();
            //_state = EnemyState.Spawning;
        }

        private void HandleSpawning()
        {
            _spriteRenderer.enabled = true;

            transform.position += _movementSpeed * Time.smoothDeltaTime * Vector3.down;

            if (transform.position.y < _verticalPosition)
            {
                _state = BossState.StartingPhaseOne;
            }

            
        }

        private void HandleStartOfPhaseOne()
        {
            _timer += Time.deltaTime;

            if (_timer > _phaseOneWaitTime)
            {
                _laserTransform.gameObject.SetActive(_hasLaserAttacks);

                _timer = 0;
                _state = BossState.PhaseOne;
            }
            
        }

        private void HandlePhaseOne()
        {
            HandleMovement();
            HandleShooting();
        }

        private void HandleStartOfPhaseTwo()
        {
            _state = BossState.StartingPhaseTwo;
        }

        private void HandlePhaseTwo()
        {
            _state = BossState.Dying;
        }

        private void HandleDying()
        {
            IsDead = true;
            _spriteRenderer.enabled = false;
            _laserTransform.gameObject.SetActive(false);

            _emitParams.position = transform.position;
            _deathParticleSystem.Emit(_emitParams, 40);

            DestroyAllBullets();
            BossDied?.Invoke();
            _state = BossState.Dead;
        }

        private void HandleMovement()
        {
            if (transform.position.x > _maxMoveRange)
            {
                _dir = -1;
            }

            if (transform.position.x < -_maxMoveRange)
            {
                _dir = 1;
            }

            transform.position += _movementSpeed * Time.smoothDeltaTime * _dir * Vector3.right;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == 10)
            {
                if (_state == BossState.PhaseOne || _state == BossState.PhaseTwo && _hp > 0)
                {
                    var bullet = other.GetComponent<BulletView>();

                    bullet.IsDead = true;

                    _hp -= 1;

                    PercentageHp?.Invoke(_hp/ (float) _maxHitPoints);

                    //_damageController.SetDamage(_hp / (float) _maxHitPoints);

                    if (_hp > 0)
                    {
                        _impactParticleSystem.Emit(_emitParams, 20);
                    }

                    if (_hp == 0)
                    {
                        //_damageController.Disable();

                        
                        GameState.AddScore(_score);
                        _state = BossState.Dying;
                    }
                }
            }
        }

        private void HandleShooting()
        {
            if (_hasNormalAttacks)
            {
                HandleNormalAttacks();
            }

            if (_hasCircleAttacks)
            {
                HandleCircleAttacks();
            }

            if (_hasLaserAttacks)
            {
                HandleLaserAttacks();
            }

            MoveBullets();

            DestroyDeadBullets();

            _deadBullets.Clear();
        }

        private void HandleLaserAttacks()
        {
            _laserAttackTimer += Time.deltaTime;

            if (_laserAttackTimer < _laserAttackCooldown)
            {
                
                return;
            }

            if (_laserCd)
            {
                _laserTransform.gameObject.SetActive(true);
                _laserCd = false;
            }
            


            _laserTransform.Rotate(Vector3.back * _laserDir * _laserRotationSpeed * Time.deltaTime, Space.Self);

            if (_laserTransform.localRotation.eulerAngles.z > 75 && _laserTransform.localRotation.eulerAngles.z < 104)
            {
                _laserTransform.localRotation = Quaternion.Euler(0, 0, 180);
                _laserDir = 1;
                _laserAttackTimer = 0;
                _laserTransform.gameObject.SetActive(false);
                _laserCd = true;
            }

            if (_laserTransform.localRotation.eulerAngles.z < 105 && _laserTransform.localRotation.eulerAngles.z > 76)
            {
                _laserDir = -1;
                _laserTransform.localRotation = Quaternion.Euler(0, 0, 0);
                _laserAttackTimer = 0;
                _laserTransform.gameObject.SetActive(false);
                _laserCd = true;
            }
        }

        private void HandleCircleAttacks()
        {
            _circleAttackTimer += Time.deltaTime;

            if (_circleAttackTimer > _circleAttackCooldown)
            {
                _circleAttackTimer = 0;

                var angle = 100 / _circleAttackCount;

                for (int i = 0; i <= _circleAttackCount; i++)
                {
                    var newBullet = Instantiate(_circleBulletPrefab, transform.position, Quaternion.identity);
                    newBullet.Direction = Quaternion.Euler(0, 0, 40 + angle * i) * Vector3.left;
                    _bullets.Add(newBullet);
                }
            }
        }

        private void HandleNormalAttacks()
        {
            _normalAttackTimer += Time.deltaTime;

            if (_normalAttackTimer > _normalAttackCooldown)
            {
                _normalAttackTimer = 0;

                for (var i = 0; i < _normalAttackCount; i++)
                {
                    var pos = transform.position + Vector3.right * _normalAttacksSpacing * (i - (_normalAttackCount - 1) / 2f);

                    _bullets.Add(Instantiate(_normalBulletPrefab, pos, Quaternion.identity));
                }
            }
        }

        private void DestroyAllBullets()
        {
            foreach (var bullet in _bullets)
            {
                bullet.IsDead = true;
                _deadBullets.Add(bullet);
            }

            DestroyDeadBullets();
        }

        private void MoveBullets()
        {
            foreach (var bullet in _bullets)
            {
                if (Vector3.Magnitude(bullet.transform.position) > _bulletMaxTravelDistance || bullet.IsDead)
                {
                    _deadBullets.Add(bullet);
                }

                switch (bullet.BulletType)
                {
                    case BulletType.Normal:
                        bullet.transform.position += _normalBulletSpeed * Time.smoothDeltaTime * Vector3.down;
                        break;
                    case BulletType.Circle:
                        bullet.transform.position += _circleBulletSpeed * Time.smoothDeltaTime * bullet.Direction;
                        break;
                }

                
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

        
    }
}