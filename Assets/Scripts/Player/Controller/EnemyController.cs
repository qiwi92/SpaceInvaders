using UnityEngine;
using Weapons.Bullet;

namespace Player.Controller
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _deathParticleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private EnemyState _enemyState = EnemyState.Spawning;

        void Update()
        {
            switch (_enemyState)
            {
                case EnemyState.Dead:
                    HandleDead();
                    break;
                case EnemyState.Spawning:
                    HandleSpawning();
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

            //_enemyState = EnemyState.Spawning;
        }

        private void HandleSpawning()
        {
            _spriteRenderer.enabled = true;
            _enemyState = EnemyState.Alive;
        }

        private void HandleAlive()
        {
            


            if (Input.GetKeyDown(KeyCode.X))
            {
                
                _enemyState = EnemyState.Dying;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            if (_enemyState == EnemyState.Alive)
            {
                var bullet = other.GetComponent<BulletView>();
                bullet.IsDead = true;
                _enemyState = EnemyState.Dying;
            }
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
            _enemyState = EnemyState.Dead;
        }
    }
}