using System.Collections;
using System.Linq;
using Enemies;
using Player.Controller;
using UniRx;
using UnityEngine;

namespace GameLogic
{
    public class GameState : MonoBehaviour
    {
        public static ReactiveProperty<int> GameTimer = new ReactiveProperty<int>(10*60);
        public static GameState Instance = null;
        public static ReactiveProperty<int> Score = new ReactiveProperty<int>(0);
        public static ReactiveProperty<int> ScoreInLastLevel = new ReactiveProperty<int>(0);
        public static ReactiveProperty<int> Level = new ReactiveProperty<int>(0);
        public static ReactiveProperty<int> Money = new ReactiveProperty<int>(0);
        public static ReactiveProperty<int> MoneyInLastLevel = new ReactiveProperty<int>(0);
        public static ReactiveProperty<string> PlayerName = new ReactiveProperty<string>("You");

        public static PlayerStatModel PlayerStats = new PlayerStatModel();


        private static readonly int[] _wayPoints = new[] {0, 3, 6, 9, 12};
        private static int _scoreAtLastWayPoint = 0;
        private static int _lastWayPoint = 0;

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

            StartCoroutine(Clock());
        }

        public static void IncreaseLevel()
        {
            
            Level.Value += 1;

            if (Level.Value > 1)
            {
                _lastWayPoint = _wayPoints.TakeWhile(p => p < Level.Value).Last();
            }

            

            
            
            if (_wayPoints.Any(x => x == Level.Value))
            {
                _scoreAtLastWayPoint = Score.Value;
            }
        }

        public static void ResetToLastWaypoint()
        {
            Score.Value = _scoreAtLastWayPoint;
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
        }

        public static void SetScore()
        {
            Score.Value += ScoreInLastLevel.Value;
        }

        public static int GetLastWaypoint()
        {
            return _lastWayPoint;
        }

        private IEnumerator Clock()
        {
            while (GameTimer.Value > 0)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("HEllo");
                GameTimer.Value -= 1;
            }
        }
    }
}