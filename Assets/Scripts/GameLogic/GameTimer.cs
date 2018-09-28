using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace GameLogic
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private Text _timerText;

        private IDisposable _disposable;
        private void Start()
        {
            _disposable = GameState.GameTimer.Subscribe(time => { _timerText.text = time.FormatTime(); });
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}