using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons.Bullet;

namespace Player.Controller
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] private float _maxMoveBound;
        [SerializeField] private float _movementSpeed;

        [SerializeField] private float _bulletSpeed;
        [SerializeField] private float _bulletMaxTravelDistance;
        [SerializeField] private BulletView _bulletPrefab;
        private readonly List<BulletView> _bullets = new List<BulletView>();
        private readonly List<BulletView> _deadBullets = new List<BulletView>();

        private PlayerState _playerState = PlayerState.Spawning;


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
            _playerState = PlayerState.Spawning;
        }

        private void HandleSpawning()
        {
            _playerState = PlayerState.Alive;
        }

        private void HandleAlive()
        {
            HandleMovementInput();
            HandleShooting();

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Player is dying!");
                _playerState = PlayerState.Dying;
            }
        }

        private void HandleDying()
        {
            _playerState = PlayerState.Dead;
        }

        private void HandleShooting()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
    }
}