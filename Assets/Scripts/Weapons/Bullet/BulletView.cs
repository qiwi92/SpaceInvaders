using UnityEngine;

namespace Weapons.Bullet
{
    public class BulletView : MonoBehaviour, IBullet
    {
        public bool IsDead { get; set; }
    }

    public interface IBullet
    {
        bool IsDead { get; set; }
    }
}