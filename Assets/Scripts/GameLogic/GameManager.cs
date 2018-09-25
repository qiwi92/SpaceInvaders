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


        private void Start()
        {
            _enemyManager.EnemiesArrievedAtPlayer += () => { _playerController.Die(); };

            _enemyManager.AllEnemiesAreDead += () =>
            {
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    SceneManager.LoadScene(2);
                });
            };

            _playerController.PlayerIsDead  += () =>
            {
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    SceneManager.LoadScene(3);
                });
            };

        }
    }
}