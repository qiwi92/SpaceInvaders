using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        private IDisposable _disposable;

        private void Start()
        {
            _disposable = GameState.ScoreInLastLevel.Subscribe(money =>
            {
                _scoreText.text = money.ToString("0");
            });
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}