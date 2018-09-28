using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameLogic
{
    
    public class ReplayHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _replay;
        [SerializeField] private GameObject _jumpBack;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Text _replayCost;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        [SerializeField] private Color _moneyColor;
        [SerializeField] private Color _cantAffordColor;

        public void Start()
        {
            var canBuy = GameState.Level
                .CombineLatest(GameState.Money, (level, money) => GetCost(level) <= money)
                .ToReactiveProperty();

            _disposables.Add(GameState.Level.Subscribe(level =>
            {
                _replayCost.text = GetCost(level).ToString("0");

                var lastLevelWasWaypoint = level == GameState.GetLastWaypoint();
                _replay.SetActive(lastLevelWasWaypoint);
                _jumpBack.SetActive(!lastLevelWasWaypoint);
                _replayButton.gameObject.SetActive(!lastLevelWasWaypoint);
            }));

            _disposables.Add(canBuy.Subscribe(x =>  _replayCost.color = x ? _moneyColor : _cantAffordColor ));

            var canBuyWeapon = new ReactiveCommand(canBuy);
            _disposables.Add(canBuyWeapon.Subscribe(_ =>
            {
                GameState.Money.Value -= GetCost(GameState.Level.Value);
                GameState.Level.Value -= 1;
                SceneManager.LoadScene(1);              
            }));

            _disposables.Add(canBuyWeapon.BindTo(_replayButton));
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private int GetCost(int level)
        {
            return level*5;
        }
    }
}