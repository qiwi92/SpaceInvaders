using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private Text _weaponLevel;
        [SerializeField] private Text _weaponAttackSpeed;
        [SerializeField] private Text _weaponUpgradeCosts;

        [SerializeField] private Button _upgradeButton;

        private ReactiveCommand CanBuyWeapon;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void Start()
        {
            var canBuy = GameState.PlayerStats.WeaponCost.Select(cost => GameState.Money.Value > cost).ToReactiveProperty();
            CanBuyWeapon = new ReactiveCommand(canBuy);

            _disposables.Add(CanBuyWeapon.Subscribe(buy =>
            {
                GameState.Money.Value -= (int) GameState.PlayerStats.WeaponCost.Value;
                GameState.PlayerStats.WeaponLevel.Value += 1;
            }));

            _disposables.Add(GameState.PlayerStats.WeaponLevel.Subscribe(lvl =>
            {
                _weaponLevel.text = lvl.ToString("0");
            }));
            _disposables.Add(GameState.PlayerStats.WeaponCooldown.Subscribe(weaponCd => _weaponAttackSpeed.text = (1/weaponCd).ToString("0.0")));
            _disposables.Add(GameState.PlayerStats.WeaponCost.Subscribe(cost => _weaponUpgradeCosts.text = cost.ToString("0")));


            _disposables.Add(CanBuyWeapon.BindTo( _upgradeButton));

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