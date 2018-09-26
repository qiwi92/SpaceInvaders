﻿using DG.Tweening;
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
            GameState.ResetValuesFromLastLevel();

            var currentLevelInfo = _levelInfos[GameState.Level.Value - 1];

            _enemyManager.Setup(currentLevelInfo);

            _enemyManager.EnemiesArrievedAtPlayer += () => { _playerController.Die(); };

            _enemyManager.AllEnemiesAreDead += () =>
            {
                //Victory
                DOVirtual.DelayedCall(2f, () =>
                {
                    SceneManager.LoadScene(2);
                    GameState.IncreaseLevel();
                });
            };

            _playerController.PlayerIsDead  += () =>
            {
                //Defeat
                DOVirtual.DelayedCall(1f, () =>
                {
                    SceneManager.LoadScene(3);
                    GameState.ResetToLastWaypoint();
                });
            };
        }
    }
}