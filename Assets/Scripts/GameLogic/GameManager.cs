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
        [SerializeField] private SpriteRenderer _backGroundSpriteRenderer;
        [SerializeField] private Sprite _defaultBackground;
        [SerializeField] private SpriteRenderer _impactLineRenderer;


        private void Start()
        {
            _impactLineRenderer.DOFade(0, 0.01f);
            
            GameState.IncreaseLevel();
            GameState.ResetValuesFromLastLevel();
   
            var currentLevelInfo = _levelInfos[GameState.Level.Value-1];
            _backGroundSpriteRenderer.sprite = currentLevelInfo.BackgroundSprite ?? _defaultBackground;

            _enemyManager.Setup(currentLevelInfo);

            _enemyManager.EnemiesArrievedAtPlayer += () =>
            {
                _impactLineRenderer.DOFade(1, 0.1f).OnComplete(() => _impactLineRenderer.DOFade(0, 2f));
                _playerController.Die();
            };

            _enemyManager.AllEnemiesAreDead += () =>
            {
                //Victory
                DOVirtual.DelayedCall(2f, () =>
                {
                    SceneManager.LoadScene(2);
                    GameState.SetScore();
                });
            };

            _playerController.PlayerIsDead  += () =>
            {
                //Defeat
                DOVirtual.DelayedCall(2f, () =>
                {
                    SceneManager.LoadScene(3);
                    GameState.SetScore();
                });
            };
        }
    }
}