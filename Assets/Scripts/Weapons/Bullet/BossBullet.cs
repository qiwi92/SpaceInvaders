using UnityEngine;

namespace Weapons.Bullet
{
    public class BossBullet : MonoBehaviour, IBullet
    {
        public Vector3 Direction { get; set; }
        public bool IsDead { get; set; }
        public BulletType BulletType;
    }

    public enum BulletType
    {
        Normal,
        Circle
    }
}