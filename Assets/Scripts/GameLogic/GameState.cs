using System.Linq;
using DG.Tweening;
using Enemies;
using Player.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class GameState : MonoBehaviour
    {
        public static GameState Instance = null;
        public static int Level = 1;
        public static int Money = 0;

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
            Level += 1;

            _lastWayPoint = _wayPoints.TakeWhile(p => p < Level).Last();
        }

        public static void ResetToLastWaypoint()
        {
            Level = _lastWayPoint;
        }

        public static void AddMoney(int coinValue)
        {
            
            Money += coinValue;

            Debug.Log("Money: " + Money);
        }
    }
}