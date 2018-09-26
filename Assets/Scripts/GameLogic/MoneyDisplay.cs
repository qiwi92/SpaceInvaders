using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace GameLogic
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private Text _moneyAmount;
        private IDisposable _disposable;

        private void Start()
        {
            _disposable =  GameState.Money.Subscribe(money =>
            {
                _moneyAmount.text = money.ToString("0");
            });
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}