using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _notificationGroup;

        [SerializeField] private Color _moneyColor;
        [SerializeField] private Color _cantAffordColor;

        [SerializeField] private Text _weaponLevel;
        [SerializeField] private Text _weaponAttackSpeed;
        [SerializeField] private Text _weaponUpgradeCosts;
        [SerializeField] private Button _weaponUpgradeButton;



        [SerializeField] private Text _bulletLevel;
        [SerializeField] private Text _bulletAmount;
        [SerializeField] private Text _bulletUpgradeCosts;
        [SerializeField] private Button _bulletUpgradeButton;

        private ReactiveCommand _canBuyWeapon;
        private ReactiveCommand _canBuyBullets;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void Start()
        {
            var canBuyWeapon = GameState.PlayerStats.WeaponCost
                .CombineLatest(GameState.Money, (cost, money) => (int) cost <=  money)
                .ToReactiveProperty();
            _canBuyWeapon = new ReactiveCommand(canBuyWeapon);

            _disposables.Add(_canBuyWeapon.Subscribe(_ =>
            {
                GameState.Money.Value -= (int)GameState.PlayerStats.WeaponCost.Value;
                GameState.PlayerStats.WeaponLevel.Value += 1;

            }));

            _disposables.Add(canBuyWeapon.Subscribe(buyable =>
                {
                    _weaponUpgradeCosts.color = buyable ? _moneyColor : _cantAffordColor;
                }));

            _disposables.Add(GameState.PlayerStats.WeaponLevel.Subscribe(lvl =>
            {
                _weaponLevel.text = (lvl +1).ToString("0");
            }));
            _disposables.Add(GameState.PlayerStats.WeaponCooldown.Subscribe(weaponCd => _weaponAttackSpeed.text = (1 / weaponCd).ToString("0.0")));
            _disposables.Add(GameState.PlayerStats.WeaponCost.Subscribe(cost => _weaponUpgradeCosts.text = cost.ToString("0")));


            _disposables.Add(_canBuyWeapon.BindTo(_weaponUpgradeButton));


            var canBuyBullets = GameState.PlayerStats.BulletCost
                .CombineLatest(GameState.Money, (cost, money) => (int)cost <= money)
                .ToReactiveProperty();
            _canBuyBullets = new ReactiveCommand(canBuyBullets);

            _disposables.Add(_canBuyBullets.Subscribe(_ =>
            {
                Debug.Log(GameState.Money.Value);
                Debug.Log(GameState.PlayerStats.BulletCost.Value);
                GameState.Money.Value -= (int) GameState.PlayerStats.BulletCost.Value;
                GameState.PlayerStats.BulletLevel.Value += 1;

            }));

            _disposables.Add(canBuyBullets.Subscribe(buyable =>
            {
                _bulletUpgradeCosts.color = buyable ? _moneyColor : _cantAffordColor;
            }));

            _disposables.Add(GameState.PlayerStats.BulletLevel.Subscribe(lvl =>
            {
                _bulletLevel.text = (lvl + 1).ToString("0");
            }));
            _disposables.Add(GameState.PlayerStats.BulletAmount.Subscribe(amount => _bulletAmount.text = amount.ToString("0")));
            _disposables.Add(GameState.PlayerStats.BulletCost.Subscribe(cost => _bulletUpgradeCosts.text =  ((int) cost).ToString("0")));


            _disposables.Add(_canBuyBullets.BindTo(_bulletUpgradeButton));

            _disposables.Add(canBuyBullets.CombineLatest(canBuyWeapon, (x,y) => x || y).Subscribe(z => _notificationGroup.alpha = z ? 1 : 0));

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