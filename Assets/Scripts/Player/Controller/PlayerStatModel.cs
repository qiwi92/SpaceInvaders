using System.Linq.Expressions;
using UniRx;

namespace Player.Controller
{
    public class PlayerStatModel
    {
        public ReactiveProperty<int> WeaponLevel = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<float> WeaponCooldown;
        public IReadOnlyReactiveProperty<float> WeaponCost;

        public ReactiveProperty<int> BulletLevel = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> BulletAmount;
        public IReadOnlyReactiveProperty<float> BulletCost;

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

        private readonly float[] _attackSpeedCosts = new[]
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

        private readonly int[] _bullets = new[]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            9,
            10,
            11,
            12
        };

        private readonly float[] _bulletCosts = new[]
        {
            1f,
            1f,
            1f,
            1f,
            2f,
            2f,
            2f,
            3f,
            4f,
            5f,
            6f,
        };

        public PlayerStatModel()
        {
            WeaponCooldown = WeaponLevel.Select(lvl => _attackCDs[lvl]).ToReactiveProperty();
            WeaponCost = WeaponLevel.Select(lvl => _attackSpeedCosts[lvl]).ToReactiveProperty();

            BulletAmount = BulletLevel.Select(lvl => _bullets[lvl]).ToReactiveProperty();
            BulletCost = BulletLevel.Select(lvl => _bulletCosts[lvl]).ToReactiveProperty();
        }
       
        public ReactiveProperty<bool> HasShield = new ReactiveProperty<bool>();
    }
}