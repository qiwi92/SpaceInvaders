using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class EndscreenStatDisplay : MonoBehaviour
    {
        [SerializeField] private Text _level;
        [SerializeField] private Text _moneyInLevel;
        [SerializeField] private Text _scoreInLevel;
        [SerializeField] private Text _multiplier;
        [SerializeField] private Text _resultingScore;
        [SerializeField] private Text _totalScore;
        
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void Start()
        {
            _disposables.Add(GameState.Level.Subscribe(level => { _level.text = "- Level " +  (level-1).ToString("0") + " -" ; }));

            _disposables.Add(GameState.MoneyInLastLevel.Subscribe(moneyInLevel => { _moneyInLevel.text = moneyInLevel.ToString("0"); }));

            _disposables.Add(GameState.ScoreInLastLevel.Subscribe(score => { _scoreInLevel.text = score.ToString("0"); }));
            _disposables.Add(GameState.Score.Subscribe(score => { _totalScore.text = score.ToString("0"); }));

            _multiplier.text = "x" + GameState.Multiplier.ToString("0.0");
            _resultingScore.text = GameState.ResultingScore.ToString("0");

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