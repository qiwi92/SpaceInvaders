using System.Linq.Expressions;
using UniRx;

namespace Player.Controller
{
    public class PlayerStatModel
    {
        public ReactiveProperty<int> WeaponLevel = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<float> WeaponCooldown;
        public IReadOnlyReactiveProperty<float> WeaponCost;

        private readonly float[] _attackCDs = new[]
        {
            0.35f,
            0.30f,
            0.26f,
            0.23f,
            0.20f,
            0.17f,
            0.15f,
            0.13f,
            0.11f,
            0.10f,
            0.9f
        };

        private readonly float[] _costs = new[]
        {
            100f,
            120f,
            144f,
            172f,
            207f,
            248f,
            298f,
            358f,
            429f,
            515f,
            600f,
        };

        public PlayerStatModel()
        {
            WeaponCooldown = WeaponLevel.Select(lvl => _attackCDs[lvl]).ToReactiveProperty();
            WeaponCost = WeaponLevel.Select(lvl => _costs[lvl]).ToReactiveProperty();
        }
       
        public ReactiveProperty<bool> HasShield = new ReactiveProperty<bool>();
    }
}