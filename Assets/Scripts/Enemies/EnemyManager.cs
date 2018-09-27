using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player.Controller;
using UnityEngine;
using DG.Tweening;


namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        private float _movemementSpeed;

        [SerializeField] private EnemyController _regularPrefab;
        [SerializeField] private EnemyController _shooterPrefab;
        [SerializeField] private EnemyController _tankPrefab;
        [SerializeField] private EnemyController _shootingTankPrefab;

        [SerializeField] private BossController _greenBoomerang;
        [SerializeField] private BossController _redCross;
        [SerializeField] private BossController _purpleNightmare;
        [SerializeField] private BossController _doomsday;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private Coin _coinPrefab;

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
        private bool _allEnemiesAreDead;

        private readonly List<int> _rewardsPerCoin = new List<int>();
        private bool _hasBoss;
        private LevelInfo _levelInfo;

        public void Setup(LevelInfo levelInfo)
        {
            _levelInfo = levelInfo;
            _movemementSpeed = levelInfo.Speed;

            _hasBoss = levelInfo.BossType != BossType.None;
            


            foreach (var direction in _path)
            {
                _pos += direction.ToVector2();
                _moveQueue.Enqueue(_pos);
            }

            for (var y = 0; y < levelInfo.LevelInfoRows.Length; y++)
            {
                var levelInfoRow = levelInfo.LevelInfoRows[y];
                for (var x = 0; x < levelInfoRow.EnemyType.Length; x++)
                {
                    var enemyInfo = levelInfoRow.EnemyType[x];
                    var enemyType = enemyInfo.EnemyType;
                    var colorType = enemyInfo.ColorType;

                    switch (enemyType)
                    {
                        case EnemyType.None:
                            break;
                        case EnemyType.Regular:
                            CreateEnemy(_regularPrefab, colorType, levelInfo, x, y);
                            break;
                        case EnemyType.Shooter:
                            CreateEnemy(_shooterPrefab, colorType, levelInfo, x, y);
                            break;
                        case EnemyType.Tank:
                            CreateEnemy(_tankPrefab, colorType, levelInfo, x, y);
                            break;
                        case EnemyType.ShootingTank:
                            CreateEnemy(_shootingTankPrefab, colorType, levelInfo, x, y);
                            break;
                    }
                }
            }


            CalculateRewardPerCoin(levelInfo.MoneyReward);

            ExecuteMove();

            EnemiesArrievedAtPlayer += () => { transform.DOKill(false); };
        }

        private void CalculateRewardPerCoin(int totalReward)
        {
            var divisor = 4;
            var numberOfEnemies = _enemies.Count;

            if (numberOfEnemies < divisor)
            {
                divisor = numberOfEnemies;
            }

            for (int i = 0; i < divisor - 1; i++)
            {
                _rewardsPerCoin.Add((int) Mathf.Floor(totalReward / (float) divisor));
            }

            _rewardsPerCoin.Add((int) Mathf.Floor(totalReward / (float) divisor) + totalReward % divisor);

            if (numberOfEnemies < divisor)
            {
                for (var i = 0; i < divisor; i++)
                {
                    var enemyController = _enemies[i];
                    var coinValue = _rewardsPerCoin[i];
                    enemyController.SetupCoin(coinValue, _coinPrefab);
                }
            }
            else
            {
                for (var i = 0; i < divisor; i++)
                {
                    var enemyController = _enemies[i* (numberOfEnemies/divisor)];
                    var coinValue = _rewardsPerCoin[i];
                    enemyController.SetupCoin(coinValue, _coinPrefab);
                }
            }
        }


        private void CreateEnemy(EnemyController enemy, ColorType color, LevelInfo levelInfo, int x, int y)
        {
            var newEnemy = Instantiate(enemy, transform);
            newEnemy.transform.position = new Vector2(x * 0.8f - 7, y * 0.8f);
            newEnemy.Setup(color, levelInfo.AttackSpeed, levelInfo.BaseHp, levelInfo.BaseHpMultiplier);

            _enemies.Add(newEnemy);
        }

        private void ExecuteMove()
        {
            if (_enemies.All(enemy => enemy.IsDead) && !_allEnemiesAreDead)
            {
                if (_hasBoss)
                {
                    SpawnBoss();
                }
                else
                {
                    AllEnemiesAreDead?.Invoke();
                }
                
                _allEnemiesAreDead = true;
            }

            if (_enemies.Any(enemy => !enemy.IsDead && enemy.transform.position.y < -3.35f))
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

                transform.DOMove(nextPos, 1/ _movemementSpeed).SetEase(Ease.Linear).OnComplete(ExecuteMove);
            }
            else
            {
                return;
            }
        }

        private void SpawnBoss()
        {
            BossController boss = null;
            switch (_levelInfo.BossType)
            {
                case BossType.GreenBoomerang:
                    boss = Instantiate(_greenBoomerang);
                    break;
                case BossType.RedCross:
                    boss = Instantiate(_redCross);
                    break;
                case BossType.PurpleNighmare:
                    boss = Instantiate(_purpleNightmare);
                    break;
                case BossType.Doomsday:
                    boss = Instantiate(_doomsday);
                    break;
            }

            Warning();
            boss.BossDied += () => { AllEnemiesAreDead?.Invoke(); };
        }

        private void Warning()
        {
            _canvasGroup.DOFade(1, 0.5f).SetEase(Ease.Linear).SetLoops(6,LoopType.Yoyo).OnComplete(() => { _canvasGroup.DOFade(0, 0.8f);});
        }
    }
}