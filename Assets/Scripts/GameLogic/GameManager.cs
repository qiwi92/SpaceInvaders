using DG.Tweening;
using Enemies;
using Player.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private PlayerController _playerController;

        [SerializeField] private LevelInfo[] _levelInfos;


        private void Start()
        {
            _enemyManager.Setup(_levelInfos[GameState.Level-1]);

            _enemyManager.EnemiesArrievedAtPlayer += () => { _playerController.Die(); };

            _enemyManager.AllEnemiesAreDead += () =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    SceneManager.LoadScene(2);
                    GameState.Level += 1;
                });
            };

            _playerController.PlayerIsDead  += () =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    SceneManager.LoadScene(3);
                });
            };

        }
    }
}