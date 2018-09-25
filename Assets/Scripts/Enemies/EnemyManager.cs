using System;
using System.Collections.Generic;
using System.Linq;
using Player.Controller;
using UnityEngine;
using DG.Tweening;


namespace Enemies
{
    public static class DirectionExtensions
    {
        public static Vector2 ToVector2(this EnemyBlockDirection direction)
        {
            switch (direction)
            {
                case EnemyBlockDirection.Down:
                    return Vector2.down*0.5f;
                case EnemyBlockDirection.Left:
                    return Vector2.left;
                case EnemyBlockDirection.Right:
                    return Vector2.right;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public enum EnemyBlockDirection
    {
        Down,
        Left,
        Right
    }

    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private float _enemyMovemementSpeed;
        [SerializeField] private LevelInfo _levelInfo;

        [SerializeField] private EnemyController _regularPrefab;
        [SerializeField] private EnemyController _shooterPrefab;
        [SerializeField] private EnemyController _tankPrefab;
        [SerializeField] private EnemyController _shootingTankPrefab;

        private readonly List<EnemyController> _enemies = new List<EnemyController>();

        public event Action EnemiesArrievedAtPlayer;
        public event Action AllEnemiesAreDead;

        private readonly List<EnemyBlockDirection> _path = new List<EnemyBlockDirection>
        {
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Down,

        };

        private readonly List<EnemyBlockDirection> _twoLineMovmentPath = new List<EnemyBlockDirection>
        {
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Left,
            EnemyBlockDirection.Down,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Right,
            EnemyBlockDirection.Down
        };

        private readonly Queue<Vector2> _moveQueue = new Queue<Vector2>();
        private Vector2 _pos = Vector2.zero;

        private void Start()
        {
            foreach (var direction in _path)
            {
                _pos += direction.ToVector2();
                _moveQueue.Enqueue(_pos);
            }

            for (var y = 0; y < _levelInfo.Rows.Length; y++)
            {
                var levelInfoRow = _levelInfo.Rows[y];
                for (var x = 0; x < levelInfoRow.EnemyType.Length; x++)
                {
                    var enemyType = levelInfoRow.EnemyType[x];

                    switch (enemyType)
                    {
                        case EnemyType.None:
                            break;
                        case EnemyType.Regular:
                            CreateEnemy(_regularPrefab, x, y);
                            break;
                        case EnemyType.Shooter:
                            CreateEnemy(_shooterPrefab, x, y);
                            break;
                        case EnemyType.Tank:
                            CreateEnemy(_tankPrefab, x, y);
                            break;
                        case EnemyType.ShootingTank:
                            CreateEnemy(_shootingTankPrefab, x, y);
                            break;
                    }
                }
            }


            ExecuteMove();

            EnemiesArrievedAtPlayer += () => { transform.DOKill(false); };

        }

        private void CreateEnemy(EnemyController enemy, int x, int y)
        {
            var newEnemy = Instantiate(enemy, transform);
            newEnemy.transform.position = new Vector2(x - 5, y);
            _enemies.Add(newEnemy);
        }

        private void ExecuteMove()
        {
            if (_enemies.All(enemy => enemy.IsDead))
            {
                AllEnemiesAreDead?.Invoke();
            }

            if (_enemies.Any(enemy => !enemy.IsDead && enemy.transform.position.y < -2.99f))
            {
                EnemiesArrievedAtPlayer?.Invoke();
            }

            if (_moveQueue.Count < 2)
            {
                foreach (var direction in _twoLineMovmentPath)
                {
                    _pos += direction.ToVector2();
                    _moveQueue.Enqueue(_pos);              
                }
            }

            if (_moveQueue.Count > 0)
            {
                var nextPos = _moveQueue.Dequeue();

                transform.DOMove(nextPos, 1/ _enemyMovemementSpeed).SetEase(Ease.Linear).OnComplete(ExecuteMove);
            }
            else
            {
                return;
            }
        }
    }
}