using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _scoreText;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void Start()
        {
            _disposables.Add(GameState.Level.Subscribe(lvl => { _levelText.text = "- level " + lvl.ToString("0") + " -"; }));
            _disposables.Add(GameState.ScoreInLastLevel.Subscribe(money => { _scoreText.text = money.ToString("0"); }));
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}