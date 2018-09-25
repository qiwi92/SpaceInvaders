using System.Collections.Generic;
using Player.Controller;
using UnityEngine;
using DG.Tweening;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyController _enemyPrefab;
        private List<EnemyController> _enemies;

        private List<Vector2> _path = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(2, 0),
            new Vector2(3, 0),
            new Vector2(3, -1),
            new Vector2(2, -1),
            new Vector2(1, -1),
            new Vector2(0, -1),
            new Vector2(-1, -1),
            new Vector2(-2, -1),
            new Vector2(-3, -1),
            new Vector2(-3, -2),
            new Vector2(-2, -2),
            new Vector2(-1, -2),
            new Vector2(-0, -2),
            new Vector2(1, -2),
            new Vector2(2, -2),
            new Vector2(3, -2),
            new Vector2(3, -3),
            new Vector2(2, -3),
            new Vector2(1, -3),
            new Vector2(0, -3),
            new Vector2(-1, -3),
            new Vector2(-2, -3),
            new Vector2(-3, -3),
        };

        private readonly Queue<Vector2> _moveQueue = new Queue<Vector2>();

        private void Start()
        {
            foreach (var node in _path)
            {
                _moveQueue.Enqueue(node);
            }


            for (var x = -5; x <= 5; x++)
            {
                for (var y = 0; y <= 4; y++)
                {
                    var newEnemy = Instantiate(_enemyPrefab, transform);
                    newEnemy.transform.position = new Vector2(x,y);
                }
            }

            ExecuteMove();
        }

        private void ExecuteMove()
        {
            if (_moveQueue.Count > 0)
            {
                var nextPos = _moveQueue.Dequeue();

                transform.DOMove(nextPos, 1).SetEase(Ease.Linear).OnComplete(ExecuteMove);
            }
            else
            {
                return;
            }
        }

    }
}