using System;
using System.Collections.Generic;
using System.Linq;
using Player.Controller;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

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
        [SerializeField] private EnemyController _enemyPrefab;
        [SerializeField] private float _enemyMovemementSpeed;
        private readonly List<EnemyController> _enemies = new List<EnemyController>();

        public event Action EnemiesArrievedAtPlayer;
        public event Action AllEnemiesAreDead;

        private readonly List<EnemyBlockDirection> _path = new List<EnemyBlockDirection>
        {
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
            EnemyBlockDirection.Down,
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


            for (var x = -5; x <= 5; x++)
            {
                for (var y = 0; y <= 4; y++)
                {
                    var newEnemy = Instantiate(_enemyPrefab, transform);
                    newEnemy.transform.position = new Vector2(x,y);
                    _enemies.Add(newEnemy);
                }
            }

            ExecuteMove();

            EnemiesArrievedAtPlayer += () => { transform.DOKill(false); };

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