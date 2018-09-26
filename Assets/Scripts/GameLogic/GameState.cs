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
    }
}