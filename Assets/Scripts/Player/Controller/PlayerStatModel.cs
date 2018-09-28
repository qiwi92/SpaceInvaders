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
            120f,
            180f,
            270f,
            405f,
            608f,
            911f,
            1367f,
            2050f,
            3075f,
            4613f,
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
            150f,
            225f,
            338f,
            506f,
            759f,
            1139f,
            1709f,
            2563f,
            3844f,
            5767f,
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