using System.Linq.Expressions;
using UniRx;

namespace Player.Controller
{
    public class PlayerStatModel
    {
        public ReactiveProperty<int> WeaponLevel = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<float> WeaponCooldown;

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

        public PlayerStatModel()
        {
            WeaponCooldown = WeaponLevel.Select(lvl => _attackCDs[lvl]).ToReactiveProperty();
        }
       
        public ReactiveProperty<bool> HasShield = new ReactiveProperty<bool>();
    }
}