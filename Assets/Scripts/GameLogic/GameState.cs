using System.Linq;
using Enemies;
using UniRx;
using UnityEngine;

namespace GameLogic
{
    public class GameState : MonoBehaviour
    {
        public static GameState Instance = null;
        public static ReactiveProperty<int> Score = new ReactiveProperty<int>(0);
        public static int ResultingScore;
        public static ReactiveProperty<int> ScoreInLastLevel = new ReactiveProperty<int>(0);
        public static ReactiveProperty<int> Level = new ReactiveProperty<int>(1);
        public static ReactiveProperty<int> Money = new ReactiveProperty<int>(0);
        public static ReactiveProperty<int> MoneyInLastLevel = new ReactiveProperty<int>(0);

        public static float Multiplier;

        private static readonly int[] _wayPoints = new[] {1, 3, 6, 9, 12};
        private static int _lastWayPoint = 1;

        [SerializeField] private LevelInfo[] _levelInfo;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public static void IncreaseLevel()
        {
            Level.Value += 1;

            _lastWayPoint = _wayPoints.TakeWhile(p => p < Level.Value).Last();
        }

        public static void ResetToLastWaypoint()
        {
            Level.Value = _lastWayPoint;
        }

        public static void AddMoney(int coinValue)
        {
            Money.Value += coinValue;
            MoneyInLastLevel.Value += coinValue;
        }

        public static void AddScore(int score)
        {
            ScoreInLastLevel.Value += score;
        }

        public static void ResetValuesFromLastLevel()
        {
            ScoreInLastLevel.Value = 0;
            MoneyInLastLevel.Value = 0;
            Multiplier = 0;
        }

        public static void SetMultiplier(float missedShotsRatio)
        {
            
            Multiplier = 2 - missedShotsRatio;
            ResultingScore =  (int) (Multiplier * ScoreInLastLevel.Value);
            Score.Value += ResultingScore;
        }
    }
}