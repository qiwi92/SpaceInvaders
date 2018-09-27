using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace GameLogic
{
    public class HighScoreDisplay : MonoBehaviour
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        [SerializeField] private Text _score;
        [SerializeField] private Text _position;
        [SerializeField] private InputField _inputField;

        private void Start()
        {
            _disposables.Add(GameState.Score.Subscribe(score => { _score.text = score.ToString("0");} ));
            _disposables.Add(GameState.Score.Subscribe(score => { _position.text = GetHighscorePosition(score); } ));
            _disposables.Add(GameState.PlayerName.Subscribe(playerName => { _inputField.text = playerName;} ));

            _inputField.onEndEdit.AddListener(playerName => { GameState.PlayerName.Value = playerName; });
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }

        private string GetHighscorePosition(int score)
        {
            var pos =  (50 * (1000 - Mathf.Log(score, 1.013f)));
            var posClamped =  Mathf.Clamp(pos, 1f, 99999f);
            return posClamped.ToString("0");
        }
    }
}